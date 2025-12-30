using System;

namespace vCashBlazorDemo.Models
{
    public class TransactionModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Type { get; set; } = "Check"; // Check, BillPay, MoneyOrder
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public bool IsVoid { get; set; }
        public decimal CashIn => IsVoid ? 0 : 
            (Type == "WireSend" ? Amount + Fee : // Send Cash: Full Amount + Fee
             Type == "WireSendDebit" ? Fee :     // Send Debit: Only Fee is Cash In
             Type == "MoneyOrder" || Type == "BillPay" ? Amount + Fee : 0);

        public decimal CashOut => IsVoid ? 0 : 
            (Type == "WireReceive" ? Amount :    // Receive: Amount is Cash Out
             Type == "Check" ? Amount - Fee : 0);
        public string Status { get; set; } = "Pending";
    }
}
