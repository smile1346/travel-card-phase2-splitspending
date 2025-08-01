namespace SplitManagement.Models
{
    public class SpendingSplit
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid TripId { get; set; }
        public decimal SplitAmount { get; set; }
        public SplitType SplitType { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PayerId { get; set; } = string.Empty; // Member who paid
        public string PayerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<SplitParticipant> Participants { get; set; } = new List<SplitParticipant>();
        public ICollection<SplitTag> Tags { get; set; } = new List<SplitTag>();
    }

    public class SplitParticipant
    {
        public Guid Id { get; set; }
        public Guid SpendingSplitId { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public decimal ShareAmount { get; set; }
        public decimal SharePercentage { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }
        
        public SpendingSplit SpendingSplit { get; set; } = null!;
    }

    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        public ICollection<SplitTag> SplitTags { get; set; } = new List<SplitTag>();
    }

    public class SplitTag
    {
        public Guid Id { get; set; }
        public Guid SpendingSplitId { get; set; }
        public Guid TagId { get; set; }
        
        public SpendingSplit SpendingSplit { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }

    public class Settlement
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string FromMemberId { get; set; } = string.Empty;
        public string FromMemberName { get; set; } = string.Empty;
        public string ToMemberId { get; set; } = string.Empty;
        public string ToMemberName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public SettlementStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SettledAt { get; set; }
    }

    public enum SplitType
    {
        Equal = 1,
        Percentage = 2,
        FixedAmount = 3,
        ByShares = 4
    }

    public enum SettlementStatus
    {
        Pending = 1,
        Completed = 2,
        Cancelled = 3
    }
}