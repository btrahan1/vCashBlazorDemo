using System;

namespace vCash.Data.Models
{
    public class TransactionHeader
    {
        public int TransactionHeaderId { get; set; }
        public Guid TransactionHeaderUid { get; set; }
        public int CustomerId { get; set; }
        public int LocationId { get; set; }
        public int GuestId { get; set; }
        public DateTime BusinessDate { get; set; }
    }

    public class TransactionDetail
    {
        public int TransactionDetailId { get; set; }
        public int TransactionHeaderId { get; set; }
        public int SequenceNumber { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTime TransDateTime { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public int CashierId { get; set; }
        public long? MakerId { get; set; }
        public int? CheckNumber { get; set; }
        public string? UtilityAccountNumber { get; set; }
        public short Void { get; set; }
        public int? XfrToFrom { get; set; }
        public decimal Tax { get; set; }
        public int? Quantity { get; set; }
        public short? Returned { get; set; }
        public int? OverrideCashierId { get; set; }
        public string? CheckType { get; set; }
        public string? Comment { get; set; }
        public Guid TransactionDetailUid { get; set; }
        public int? SourceTransactionTypeId { get; set; }
        public int? DropEnvelopeNumber { get; set; }
        public string? JSonData { get; set; }
        public decimal? OriginalFee { get; set; }
        public decimal? OriginalFeePct { get; set; }
        public string? CheckNum { get; set; }
        public short KeyedMicr { get; set; }
    }

    public class TransactionCheckImage
    {
        public int TransactionCheckId { get; set; }
        public int TransactionDetailId { get; set; }
        public int CustomerId { get; set; }
        public int LocationId { get; set; }
        public decimal? CheckAmount { get; set; }
        public string? BankRtn { get; set; }
        public string? AccountNumber { get; set; }
        public long? CheckNumber { get; set; }
        public byte[]? CheckImageFront { get; set; }
        public byte[]? CheckImageBack { get; set; }
        public Guid? CheckImageGuid { get; set; }
    }
}
