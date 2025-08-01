using SplitManagement.Models;

namespace SplitManagement.DTOs
{
    public class CreateSplitRequest
    {
        public Guid TransactionId { get; set; }
        public Guid TripId { get; set; }
        public decimal TotalAmount { get; set; }
        public SplitType SplitType { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PayerId { get; set; } = string.Empty;
        public string PayerName { get; set; } = string.Empty;
        public List<SplitParticipantDto> Participants { get; set; } = new();
        public List<string> TagNames { get; set; } = new();
    }

    public class SplitParticipantDto
    {
        public string MemberId { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public decimal? ShareAmount { get; set; }
        public decimal? SharePercentage { get; set; }
        public int? Shares { get; set; }
    }

    public class SplitResponse
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid TripId { get; set; }
        public decimal SplitAmount { get; set; }
        public SplitType SplitType { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PayerId { get; set; } = string.Empty;
        public string PayerName { get; set; } = string.Empty;
        public List<ParticipantResponse> Participants { get; set; } = new();
        public List<TagResponse> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class ParticipantResponse
    {
        public Guid Id { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public decimal ShareAmount { get; set; }
        public decimal SharePercentage { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class TagResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class SettlementResponse
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

    public class CreateTagRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MarkPaidRequest
    {
        public Guid ParticipantId { get; set; }
    }
}