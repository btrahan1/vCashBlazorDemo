using Blazored.LocalStorage;
using vCash.Data.Models;
using vCash.Data.Repositories;

namespace vCashBlazorDemo.Services.Local
{
    public class LocalTransactionRepository : ITransactionRepository
    {
        private readonly ILocalStorageService _localStorage;
        private const string Key = "vCash_Transactions";

        public LocalTransactionRepository(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private async Task<List<TransactionDetail>> LoadHelperAsync()
        {
            // We store Details since that has the core info
            var list = await _localStorage.GetItemAsync<List<TransactionDetail>>(Key);
             if (list == null) list = new List<TransactionDetail>();
             return list;
        }

        public async Task<Bank?> GetBankAsync(string routingNumber)
        {
            // Mock: Return a generic bank
            return await Task.FromResult(new Bank { BankId=1, RoutingNumber=routingNumber, CustomerName="Demo Bank", LimitAmount=5000 });
        }

        public async Task<IEnumerable<GuestTransactionHistory>> GetHistoryByGuestIdAsync(int guestId)
        {
            // Currently our "TransactionDetail" mock storage doesn't link perfectly efficiently, but for demo:
            var all = await LoadHelperAsync();
            // Filter logic would usually join Header, but we'll simplified/mock it or assume we store "History" objects separately.
            // Actually, `TransactionDetail` doesn't have `GuestId` (Header does).
            // For this Demo simplification: We won't implement full relational DB logic in LocalStorage.
            // We will just return a seeded static list for "History" to look good, 
            // PLUS any *new* transactions we might have pushed to a separate "HistoryKey".
            
            var historyKey = $"vCash_History_{guestId}";
            var history = await _localStorage.GetItemAsync<List<GuestTransactionHistory>>(historyKey);
            
            if (history == null || !history.Any())
            {
                // Return seed if empty
                return new List<GuestTransactionHistory>
                {
                    new GuestTransactionHistory { TransDateTime = DateTime.Now.AddDays(-2), Amount = 200, Fee = 5, IsCheck = true, LocationName = "Demo Store" },
                    new GuestTransactionHistory { TransDateTime = DateTime.Now.AddDays(-30), Amount = 500, Fee = 10, IsCheck = true, LocationName = "Demo Store" }
                };
            }
            return history;
        }

        public async Task<IEnumerable<GuestTransactionHistory>> GetHistoryByMakerIdAsync(int makerId)
        {
             return new List<GuestTransactionHistory>(); // Empty for demo
        }

        public async Task<Maker?> GetMakerAsync(string routingNumber, string accountNumber, int locationId)
        {
            return new Maker { MakerId=1, Name="Demo Maker Inc.", AccountNumber=accountNumber, RoutingNumber=routingNumber };
        }

        public async Task<IEnumerable<Maker>> GetMakersByLocationAsync(int locationId)
        {
            return new List<Maker>();
        }

        public async Task SaveTransactionAsync(TransactionHeader header, IEnumerable<TransactionDetail> details, IEnumerable<TransactionCheckImage> checkImages)
        {
            // 1. Save main details for logging
            var all = await LoadHelperAsync();
            all.AddRange(details);
            await _localStorage.SetItemAsync(Key, all);

            // 2. Update Guest History (Persist this so the UI updates)
            var historyKey = $"vCash_History_{header.GuestId}";
            var history = await _localStorage.GetItemAsync<List<GuestTransactionHistory>>(historyKey) ?? new List<GuestTransactionHistory>();
            
            foreach (var d in details)
            {
                history.Insert(0, new GuestTransactionHistory 
                { 
                    TransDateTime = DateTime.Now, 
                    Amount = d.Amount, 
                    Fee = d.Fee, 
                    LocationName = "Demo Location",
                    IsCheck = true // Assuming check for demo
                });
            }
            await _localStorage.SetItemAsync(historyKey, history);
        }
    }
}
