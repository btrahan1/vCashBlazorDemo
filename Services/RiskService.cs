using System;
using System.Linq;
using System.Threading.Tasks;
using vCash.Data.Models;
using vCash.Data.Repositories;

namespace vCashBlazorDemo.Services
{
    public class RiskResult
    {
        public bool NeedsRiskManagement { get; set; }
        public bool CallMaker { get; set; }
        public bool CallBank { get; set; }
        public bool CallCorporate { get; set; }
        public string Reason { get; set; } = "";
        public string GeneratedCode { get; set; } = "";
        public string ExpectedAnswer { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class RiskService
    {
        private readonly ITransactionRepository _repository;
        private const decimal SYSTEM_OFFICE_LIMIT = 2500.00m;

        public RiskService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<RiskResult> AnalyzeRiskAsync(decimal amount, Guest? guest, Maker? maker, Bank? bank)
        {
            var result = new RiskResult { Amount = amount };
            
            // 1. Amount Checks
            if (amount > SYSTEM_OFFICE_LIMIT)
            {
                result.CallCorporate = true;
                result.Reason += $"Amount {amount:C} exceeds Office Limit {SYSTEM_OFFICE_LIMIT:C}. ";
            }

            if (bank != null && amount > bank.LimitAmount)
            {
                result.CallBank = true;
                result.Reason += $"Amount {amount:C} exceeds Bank Limit {bank.LimitAmount:C}. ";
            }

            if (maker != null && maker.LimitAmount.HasValue && amount > maker.LimitAmount.Value)
            {
                result.CallMaker = true;
                result.Reason += $"Amount {amount:C} exceeds Maker Limit {maker.LimitAmount:C}. ";
            }

            // 2. Velocity Checks
            if (maker != null)
            {
                var history = await _repository.GetHistoryByMakerIdAsync((int)maker.MakerId);
                var recentCount = history.Count(); 
                
                if (recentCount == 0 && amount > 500)
                {
                     result.CallMaker = true;
                     result.Reason += "New Maker (No History). ";
                }
            }

            // 3. Consolidate Risk
            if (result.CallBank || result.CallMaker || result.CallCorporate)
            {
                result.NeedsRiskManagement = true;
                result.GeneratedCode = GenerateCode();
                result.ExpectedAnswer = CalculateExpectedCode(result.GeneratedCode);
            }

            return result;
        }

        private string GenerateCode()
        {
            Random r = new Random();
            string astr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string tstr = r.Next(1000, 9999).ToString();
            int charIndex = r.Next(0, 26);
            string charStr = astr.Substring(charIndex, 1);
            return tstr + charStr;
        }

        public string CalculateExpectedCode(string generatedCode)
        {
             if (string.IsNullOrEmpty(generatedCode) || generatedCode.Length < 5) return "ERROR";

            string astr = "ABCDEFGHIJKLMNOPQRSTUVWXYZA";
            
            string fstr = generatedCode.Substring(4, 1);
            int pos = astr.IndexOf(fstr);
            if (pos == -1) return "ERROR";

            string nstr = astr.Substring(pos + 1, 1);

            string dtstr = DateTime.Now.ToString("MM-dd-yy"); 
            int dtpart1 = int.Parse(dtstr.Substring(0, 1));
            int dtpart2 = int.Parse(dtstr.Substring(1, 1));
            int dtpart3 = int.Parse(dtstr.Substring(3, 1)); 
            int dtpart4 = int.Parse(dtstr.Substring(4, 1));
            
            int sum1 = dtpart1 + dtpart2 + dtpart3 + dtpart4;
            string sstr = sum1.ToString("D2");
            nstr = nstr + sstr; 

            int lp1 = int.Parse(generatedCode.Substring(0, 1));
            int lp2 = int.Parse(generatedCode.Substring(1, 1));
            string lpstr = (lp1 + lp2).ToString("D2");

            return nstr + lpstr; 
        }
    }
}
