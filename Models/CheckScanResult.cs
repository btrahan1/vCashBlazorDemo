namespace vCashBlazorDemo.Models
{
    public class CheckScanResult
    {
        public decimal Amount { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string MakerName { get; set; }
    }
}
