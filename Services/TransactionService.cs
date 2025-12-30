using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vCashBlazorDemo.Models;
using vCash.Data.Repositories;
using vCash.Data.Models;
using vCashBlazorDemo.Services;

namespace vCashBlazorDemo.Services
{
    // Fix CS0246 - Missing Definition
    public class TransactionTotals
    {
        public decimal TotalFees { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal NetCash { get; set; }
    }

    public class TransactionService
    {
        private readonly HardwareService _hardwareService; 
        private readonly ITransactionRepository _repository;
        private readonly IDispenserRepository _dispenserRepository;
        private List<TransactionModel> _cart = new List<TransactionModel>();
        
        public event Action? OnChange;

        public TransactionService(HardwareService hardwareService, ITransactionRepository repository, IDispenserRepository dispenserRepository)
        {
            _hardwareService = hardwareService;
            _repository = repository;
            _dispenserRepository = dispenserRepository;
        }

        public IEnumerable<TransactionModel> GetCart() => _cart;

        // State Persistence
        public GuestModel? CurrentGuest { get; private set; }
        public string ActiveTab { get; private set; } = "Checks";

        public void SetGuest(GuestModel guest)
        {
            CurrentGuest = guest;
            NotifyStateChanged();
        }

        public void SetActiveTab(string tab)
        {
            ActiveTab = tab;
            NotifyStateChanged();
        }

        public TransactionTotals GetTotals()
        {
            return new TransactionTotals
            {
                TotalFees = _cart.Sum(x => x.Fee),
                CashIn = _cart.Sum(x => x.CashIn),
                CashOut = _cart.Sum(x => x.CashOut),
                NetCash = _cart.Sum(x => x.CashIn) - _cart.Sum(x => x.CashOut)
            };
        }

        public async Task<(bool Success, string Message, BillCounts? ChangeCounts)> ProcessCashTransaction(BillCounts paidInCounts)
        {
            if (!_cart.Any()) return (false, "Cart is empty", null);

            var totals = GetTotals();
            decimal netDue = totals.NetCash; // Positive = Customer Owes, Negative = Customer Receives

            // 1. Calculate Change / Paid Out
            decimal changeAmount = 0;
            BillCounts paidOutCounts = new BillCounts();

            if (netDue > 0)
            {
                // Customer Owes Money
                if (paidInCounts.Total < netDue)
                {
                    return (false, $"Insufficient Funds. Due: {netDue:C}, Paid: {paidInCounts.Total:C}", null);
                }
                changeAmount = paidInCounts.Total - netDue;
            }
            else
            {
                // Customer Receives Money (NetDue is negative)
                changeAmount = paidInCounts.Total + Math.Abs(netDue);
            }

            // 2. Calculate Bill Breakdown for Paid Out (Greedy)
            paidOutCounts = CalculateBillBreakdown(changeAmount);

            // 3. Update Dispenser (Database)
            try 
            {
                // Hardcoded Context for POC: Location 15, Today
                var dispenser = await _dispenserRepository.GetDispenserAsync(15, DateTime.Today);
                
                if (dispenser == null)
                {
                     return (false, "Dispenser not found for today.", null);
                }

                // Update Counts (Incrementing existing values)
                dispenser.PaidIn1s += paidInCounts.Ones;
                dispenser.PaidIn5s += paidInCounts.Fives;
                dispenser.PaidIn10s += paidInCounts.Tens;
                dispenser.PaidIn20s += paidInCounts.Twenties;
                dispenser.PaidIn50s += paidInCounts.Fifties;
                dispenser.PaidIn100s += paidInCounts.Hundreds;

                dispenser.PaidOut1s += paidOutCounts.Ones;
                dispenser.PaidOut5s += paidOutCounts.Fives;
                dispenser.PaidOut10s += paidOutCounts.Tens;
                dispenser.PaidOut20s += paidOutCounts.Twenties;
                dispenser.PaidOut50s += paidOutCounts.Fifties;
                dispenser.PaidOut100s += paidOutCounts.Hundreds;

                await _dispenserRepository.UpdateDispenserCountsAsync(dispenser);
            }
            catch (Exception ex)
            {
                 return (false, $"Dispenser Update Failed: {ex.Message}", null);
            }

            // 4. Save Transaction Header/Details
            var result = await SubmitTransactionAsync(CurrentGuest?.GuestId ?? 1, 1); // Use CurrentGuest, default to 1 if null (Safety)
            if (!result.Success) return (false, result.Message, null);

            return (true, "Success", paidOutCounts);
        }

        private BillCounts CalculateBillBreakdown(decimal amount)
        {
            var counts = new BillCounts();
            int remaining = (int)amount; // Assuming whole dollars for bills for now. Coins are ignored in this phase?

            counts.Hundreds = remaining / 100; remaining %= 100;
            counts.Fifties = remaining / 50; remaining %= 50;
            counts.Twenties = remaining / 20; remaining %= 20;
            counts.Tens = remaining / 10; remaining %= 10;
            counts.Fives = remaining / 5; remaining %= 5;
            counts.Ones = remaining / 1; remaining %= 1;

            return counts;
        }

        public async Task<(bool Success, string Message)> SubmitTransactionAsync(int guestId, int cashierId)
        {
            if (!_cart.Any()) return (false, "Cart is empty");
            
            var totals = GetTotals();

            // 1. Create Header
            var header = new vCash.Data.Models.TransactionHeader
            {
                TransactionHeaderUid = Guid.NewGuid(),
                CustomerId = 1, 
                LocationId = 15, // Fixed per user config
                GuestId = guestId,
                BusinessDate = DateTime.Today
            };

            // 2. Map Details
            var details = new List<vCash.Data.Models.TransactionDetail>();
            var checkImages = new List<vCash.Data.Models.TransactionCheckImage>();

            int sequence = 1;

            foreach (var item in _cart)
            {
                // Map Transaction Type
                int typeId = item.Type switch
                {
                    "Check" => 1,
                    "MoneyOrder" => 2,
                    "Wire" => 3, // Legacy
                    "WireSend" => 10, // Assuming IDs for new types
                    "WireSendDebit" => 11,
                    "WireReceive" => 12,
                    _ => 0
                };

                // ... (Detail mapping) ...
                 var detail = new vCash.Data.Models.TransactionDetail
                {
                    SequenceNumber = sequence,
                    TransactionTypeId = typeId,
                    TransDateTime = DateTime.Now,
                    Amount = item.Amount,
                    Fee = item.Fee,
                    CashierId = cashierId,
                    TransactionDetailUid = Guid.NewGuid(),
                    Void = (short)(item.IsVoid ? 1 : 0),
                    Tax = 0,
                    Quantity = 1,
                    CheckNumber = typeId == 1 ? 1001 : null,
                    CheckType = typeId == 1 ? "Personal" : null,
                    SourceTransactionTypeId = 0,
                    KeyedMicr = 0,
                    Comment = item.Status,
                    JSonData = "{}" 
                };

                details.Add(detail);

                // 3. Map Images (Only for Checks)
                if (typeId == 1)
                {
                    // Mock Image Logic
                    checkImages.Add(new vCash.Data.Models.TransactionCheckImage
                    {
                        TransactionDetailId = sequence, // Convention
                        CustomerId = 1,
                        LocationId = 1,
                        CheckAmount = item.Amount,
                        CheckNumber = 1001,
                        CheckImageFront = new byte[0], 
                        CheckImageBack = new byte[0],
                        CheckImageGuid = Guid.NewGuid()
                    });
                }

                sequence++;
            }

            // 4. Persist
            try
            {
                await _repository.SaveTransactionAsync(header, details, checkImages);
                Clear(); // Reset UI
                return (true, "Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction Save Failed: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public void AddCheck(decimal amount)
        {
            // Logic: Fee is 1% or $5 minimum
            var fee = Math.Max(5.00m, amount * 0.01m);
            _cart.Add(new TransactionModel 
            { 
                Type = "Check", 
                Amount = amount, 
                Fee = fee,
                Status = "Scanned"
            });
            NotifyStateChanged();
        }

        public void AddBillPay(decimal amount)
        {
            // Logic: Fee is flat $2.50
            _cart.Add(new TransactionModel
            {
                Type = "BillPay",
                Amount = amount,
                Fee = 2.50m,
                Status = "Pending"
            });
            NotifyStateChanged();
        }
        public void AddMoneyOrder(decimal amount, string payee)
        {
            // Logic: Fee is flat $1.59 (per SQL TransactionTypes ID 11)
            // Note: User SQL said ID 11 = "Money Order " with Fee 1.59
            _cart.Add(new TransactionModel 
            { 
                Type = "MoneyOrder", // We might want to switch to Int ID eventually
                Amount = amount, 
                Fee = 1.59m, 
                Status = $"Payee: {payee}"
            });
            NotifyStateChanged();
        }

        public void AddWireSend(decimal amount, decimal fee, decimal total)
        {
            // Send Cash: Customer Pays (Cash In) -> Amount + Fee
            // We use the computed Total passed from UI or recompute it
            _cart.Add(new TransactionModel 
            { 
                Type = "WireSend", 
                Amount = amount, 
                Fee = fee,
                Status = "Western Union Send" 
            });
            NotifyStateChanged();
        }

        public void AddWireSendDebit(decimal amount, decimal fee, decimal total)
        {
            // Send Debit: Customer pays Principal via Card, Fee via Cash?
            // "only add the FEE to the cart" implies CashIn = Fee.
            // We set Type = WireSendDebit to handle this special logic in Model.
            _cart.Add(new TransactionModel 
            { 
                Type = "WireSendDebit", 
                Amount = amount, 
                Fee = fee,
                Status = "Western Union Debit Send" 
            });
            NotifyStateChanged();
        }

        public void AddWireReceiveCheckless(decimal amount, string mtcn)
        {
            // Receive Checkless: Customer Receives Cash (Cash Out). No Fee.
            _cart.Add(new TransactionModel 
            { 
                Type = "WireReceive", 
                Amount = amount, 
                Fee = 0,
                Status = $"MTCN: {mtcn}" 
            });
            NotifyStateChanged();
        }

        public void VoidTransaction(Guid id)
        {
            var item = _cart.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.IsVoid = true;
                item.Status = "Voided";
                NotifyStateChanged();
            }
        }

        public void Clear()
        {
            _cart.Clear();
            CurrentGuest = null;
            ActiveTab = "Checks";
            NotifyStateChanged();
        }

        public async Task<vCash.Data.Models.Dispenser?> GetDispenserStatusAsync()
        {
            // Fetch current status for Location 15 / Today
            return await _dispenserRepository.GetDispenserAsync(15, DateTime.Today);
        }

        public async Task<Maker?> GetMakerAsync(string rt, string acct, int locationId = 15) => await _repository.GetMakerAsync(rt, acct, locationId);
        public async Task<Bank?> GetBankAsync(string rt) => await _repository.GetBankAsync(rt);
        public async Task<IEnumerable<Maker>> GetRandomMakersAsync(int locationId = 15) => await _repository.GetMakersByLocationAsync(locationId);

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
