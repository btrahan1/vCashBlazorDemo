using System;

namespace vCash.Data.Models
{
    public class GuestTransactionHistory
    {
        public DateTime TransDateTime { get; set; }
        public string LocationName { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string? CheckNumber { get; set; }
        public string? CheckType { get; set; }
        public string CashierFirstName { get; set; } = "";
        public bool IsCheck { get; set; }
        public bool IsReturned { get; set; }
    }

    public class Bank
    {
        public int BankId { get; set; } 
        public string RoutingNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal LimitAmount { get; set; } = 3000.00m; 
        public bool Blocked { get; set; }
    }

    public class Maker
    {
        public long MakerId { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public decimal? LimitAmount { get; set; }
        public bool Blocked { get; set; }
        public string Comment { get; set; }
        public DateTime? DateVerified { get; set; }
        public int LocationId { get; set; }
    }

    public class BillCounts
    {
        public int Ones { get; set; }
        public int Fives { get; set; }
        public int Tens { get; set; }
        public int Twenties { get; set; }
        public int Fifties { get; set; }
        public int Hundreds { get; set; }

        public decimal Total => (Ones * 1) + (Fives * 5) + (Tens * 10) + (Twenties * 20) + (Fifties * 50) + (Hundreds * 100);
    }
}
