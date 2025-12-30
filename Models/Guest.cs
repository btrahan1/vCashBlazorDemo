using System;

namespace vCash.Data.Models
{
    public class Guest
    {
        public int GuestId { get; set; }
        public int CustomerId { get; set; }
        public Guid GuestUid { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }
        public DateTime? BirthDate { get; set; }
        public string SSN { get; set; }
        public string DrLicNbr { get; set; }
        public string DrLicState { get; set; }
        public string Sex { get; set; } // 'M' or 'F'
        
        // Contact Info (Flattened in Legacy DB)
        public string PhoneNum1 { get; set; }
        public string PhoneType1 { get; set; }
        public string PhoneNum2 { get; set; }
        public string PhoneType2 { get; set; }
        
        public decimal AccountLimit { get; set; }
        public bool Blocked { get; set; }
        public DateTime? BlockDate { get; set; }
        public string Comment { get; set; }
        public string EmailAddress { get; set; }
        
        // Referral & Status
        public int? ReferralGuestId { get; set; }
        public decimal ReferralCredit { get; set; }
        public DateTime? DateLastVisit { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
