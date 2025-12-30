using System;

namespace vCashBlazorDemo.Models
{
    public class GuestModel
    {
        public int GuestId { get; set; } // DB PK
        public Guid Id { get; set; } = Guid.NewGuid(); // UI/Transient ID
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string FullName => $"{LastName}, {FirstName}";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Zip { get; set; } = "";
        public string DLNumber { get; set; } = "";
        public string DLState { get; set; } = "";
        public DateTime? BirthDate { get; set; }
        public string SSN { get; set; } = "";
        public string Sex { get; set; } = "M"; // Default?
        
        public string Phone1 { get; set; } = "";
        public string Phone2 { get; set; } = "";
        
        public string PhotoUrl { get; set; } = "";
        public string PhotoIdUrl { get; set; } = ""; // Scanned ID Image
        public bool IsBanned { get; set; }
        public string Notes { get; set; } = "";
        public decimal AccountLimit { get; set; }
    }
}
