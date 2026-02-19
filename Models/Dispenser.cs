using System;

namespace vCash.Data.Models
{
    public class Dispenser
    {
        public int DispenserIdentityId { get; set; }
        public int DispenserId { get; set; }
        public int LocationId { get; set; }
        public int DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public DateTime BusinessDate { get; set; }

        // Opening Balance
        public int Open1s { get; set; }
        public int Open5s { get; set; }
        public int Open10s { get; set; }
        public int Open20s { get; set; }
        public int Open50s { get; set; }
        public int Open100s { get; set; }

        // Paid In (Customer Pays)
        public int PaidIn1s { get; set; }
        public int PaidIn5s { get; set; }
        public int PaidIn10s { get; set; }
        public int PaidIn20s { get; set; }
        public int PaidIn50s { get; set; }
        public int PaidIn100s { get; set; }

        // Paid Out (Dispensed/Change)
        public int PaidOut1s { get; set; }
        public int PaidOut5s { get; set; }
        public int PaidOut10s { get; set; }
        public int PaidOut20s { get; set; }
        public int PaidOut50s { get; set; }
        public int PaidOut100s { get; set; }

        // Courier Transfers (Loads/Pickups)
        public int Load1s { get; set; }
        public int Load5s { get; set; }
        public int Load10s { get; set; }
        public int Load20s { get; set; }
        public int Load50s { get; set; }
        public int Load100s { get; set; }

        public int Pickup1s { get; set; }
        public int Pickup5s { get; set; }
        public int Pickup10s { get; set; }
        public int Pickup20s { get; set; }
        public int Pickup50s { get; set; }
        public int Pickup100s { get; set; }

        // Current Inventory
        public int Current1s { get; set; }
        public int Current5s { get; set; }
        public int Current10s { get; set; }
        public int Current20s { get; set; }
        public int Current50s { get; set; }
        public int Current100s { get; set; }

        public decimal TotalInventoryValue =>
            (Current1s * 1) +
            (Current5s * 5) +
            (Current10s * 10) +
            (Current20s * 20) +
            (Current50s * 50) +
            (Current100s * 100);

        public void SyncInventory()
        {
            Current1s = Open1s + PaidIn1s - PaidOut1s + Load1s - Pickup1s;
            Current5s = Open5s + PaidIn5s - PaidOut5s + Load5s - Pickup5s;
            Current10s = Open10s + PaidIn10s - PaidOut10s + Load10s - Pickup10s;
            Current20s = Open20s + PaidIn20s - PaidOut20s + Load20s - Pickup20s;
            Current50s = Open50s + PaidIn50s - PaidOut50s + Load50s - Pickup50s;
            Current100s = Open100s + PaidIn100s - PaidOut100s + Load100s - Pickup100s;
        }
    }
}
