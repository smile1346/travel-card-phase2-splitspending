using Microsoft.EntityFrameworkCore;
using SplitManagement.Data;
using SplitManagement.DTOs;
using SplitManagement.Models;
using SplitManagement.Repositories;

namespace SplitManagement.Services
{
    public class SplitService : ISplitService
    {
        private readonly ISplitRepository _splitRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public SplitService(
            ISplitRepository splitRepository,
            ITagRepository tagRepository,
            IHttpClientFactory httpClientFactory)
        {
            _splitRepository = splitRepository;
            _tagRepository = tagRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SplitResponse> CreateSplitAsync(CreateSplitRequest request)
        {
            // Validate trip exists
            var tripClient = _httpClientFactory.CreateClient("TripService");
            var tripResponse = await tripClient.GetAsync($"/api/trips/{request.TripId}");
            
            if (!tripResponse.IsSuccessStatusCode)
            {
                throw new ArgumentException("Trip not found");
            }

            var split = new SpendingSplit
            {
                TransactionId = request.TransactionId,
                TripId = request.TripId,
                SplitAmount = request.TotalAmount,
                SplitType = request.SplitType,
                Currency = request.Currency,
                PayerId = request.PayerId,
                PayerName = request.PayerName
            };

            // Calculate participant shares based on split type
            split.Participants = await CalculateParticipantShares(request);

            // Handle tags
            if (request.TagNames.Any())
            {
                var tags = await _tagRepository.GetOrCreateTagsAsync(request.TagNames);
                split.Tags = tags.Select(t => new SplitTag { TagId = t.Id }).ToList();
            }

            var createdSplit = await _splitRepository.CreateAsync(split);
            return MapToSplitResponse(createdSplit);
        }

        public async Task<SplitResponse?> GetSplitByIdAsync(Guid id)
        {
            var split = await _splitRepository.GetByIdAsync(id);
            return split != null ? MapToSplitResponse(split) : null;
        }

        public async Task<IEnumerable<SplitResponse>> GetSplitsByTripIdAsync(Guid tripId)
        {
            var splits = await _splitRepository.GetByTripIdAsync(tripId);
            return splits.Select(MapToSplitResponse);
        }

        public async Task<SplitResponse> UpdateSplitAsync(Guid id, CreateSplitRequest request)
        {
            var split = await _splitRepository.GetByIdAsync(id);
            if (split == null)
                throw new ArgumentException("Split not found");

            split.SplitAmount = request.TotalAmount;
            split.SplitType = request.SplitType;
            split.Currency = request.Currency;
            
            // Recalculate participant shares
            split.Participants = await CalculateParticipantShares(request);

            var updatedSplit = await _splitRepository.UpdateAsync(split);
            return MapToSplitResponse(updatedSplit);
        }

        public async Task DeleteSplitAsync(Guid id)
        {
            await _splitRepository.DeleteAsync(id);
        }

        public async Task MarkParticipantPaidAsync(Guid participantId)
        {
            await _splitRepository.MarkParticipantPaidAsync(participantId);
        }

        private async Task<List<SplitParticipant>> CalculateParticipantShares(CreateSplitRequest request)
        {
            var participants = new List<SplitParticipant>();

            switch (request.SplitType)
            {
                case SplitType.Equal:
                    var equalShare = request.TotalAmount / request.Participants.Count;
                    participants = request.Participants.Select(p => new SplitParticipant
                    {
                        MemberId = p.MemberId,
                        MemberName = p.MemberName,
                        ShareAmount = equalShare,
                        SharePercentage = 100m / request.Participants.Count
                    }).ToList();
                    break;

                case SplitType.Percentage:
                    participants = request.Participants.Select(p => new SplitParticipant
                    {
                        MemberId = p.MemberId,
                        MemberName = p.MemberName,
                        ShareAmount = request.TotalAmount * (p.SharePercentage ?? 0) / 100,
                        SharePercentage = p.SharePercentage ?? 0
                    }).ToList();
                    break;

                case SplitType.FixedAmount:
                    participants = request.Participants.Select(p => new SplitParticipant
                    {
                        MemberId = p.MemberId,
                        MemberName = p.MemberName,
                        ShareAmount = p.ShareAmount ?? 0,
                        SharePercentage = (p.ShareAmount ?? 0) / request.TotalAmount * 100
                    }).ToList();
                    break;

                case SplitType.ByShares:
                    var totalShares = request.Participants.Sum(p => p.Shares ?? 0);
                    participants = request.Participants.Select(p => new SplitParticipant
                    {
                        MemberId = p.MemberId,
                        MemberName = p.MemberName,
                        ShareAmount = request.TotalAmount * (p.Shares ?? 0) / totalShares,
                        SharePercentage = (decimal)(p.Shares ?? 0) / totalShares * 100
                    }).ToList();
                    break;
            }

            return participants;
        }

        private static SplitResponse MapToSplitResponse(SpendingSplit split)
        {
            return new SplitResponse
            {
                Id = split.Id,
                TransactionId = split.TransactionId,
                TripId = split.TripId,
                SplitAmount = split.SplitAmount,
                SplitType = split.SplitType,
                Currency = split.Currency,
                PayerId = split.PayerId,
                PayerName = split.PayerName,
                Participants = split.Participants.Select(p => new ParticipantResponse
                {
                    Id = p.Id,
                    MemberId = p.MemberId,
                    MemberName = p.MemberName,
                    ShareAmount = p.ShareAmount,
                    SharePercentage = p.SharePercentage,
                    IsPaid = p.IsPaid,
                    PaidAt = p.PaidAt
                }).ToList(),
                Tags = split.Tags.Select(st => new TagResponse
                {
                    Id = st.Tag.Id,
                    Name = st.Tag.Name,
                    Color = st.Tag.Color,
                    Description = st.Tag.Description
                }).ToList(),
                CreatedAt = split.CreatedAt
            };
        }
    }

    public class SettlementService : ISettlementService
    {
        private readonly SplitDbContext _context;

        public SettlementService(SplitDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SettlementResponse>> CalculateSettlementsAsync(Guid tripId)
        {
            // Get all splits for the trip
            var splits = await _context.SpendingSplits
                .Include(s => s.Participants)
                .Where(s => s.TripId == tripId)
                .ToListAsync();

            // Calculate net balance for each member
            var memberBalances = new Dictionary<string, decimal>();

            foreach (var split in splits)
            {
                // Add amount paid by payer
                if (!memberBalances.ContainsKey(split.PayerId))
                    memberBalances[split.PayerId] = 0;
                memberBalances[split.PayerId] += split.SplitAmount;

                // Subtract each participant's share
                foreach (var participant in split.Participants)
                {
                    if (!memberBalances.ContainsKey(participant.MemberId))
                        memberBalances[participant.MemberId] = 0;
                    memberBalances[participant.MemberId] -= participant.ShareAmount;
                }
            }

            // Generate settlements using simplified debt algorithm
            var settlements = new List<Settlement>();
            var creditors = memberBalances.Where(mb => mb.Value > 0).OrderByDescending(mb => mb.Value).ToList();
            var debtors = memberBalances.Where(mb => mb.Value < 0).OrderBy(mb => mb.Value).ToList();

            foreach (var debtor in debtors)
            {
                var remainingDebt = Math.Abs(debtor.Value);
                
                foreach (var creditor in creditors)
                {
                    if (remainingDebt <= 0 || memberBalances[creditor.Key] <= 0) continue;

                    var settlementAmount = Math.Min(remainingDebt, memberBalances[creditor.Key]);
                    
                    settlements.Add(new Settlement
                    {
                        Id = Guid.NewGuid(),
                        TripId = tripId,
                        FromMemberId = debtor.Key,
                        ToMemberId = creditor.Key,
                        Amount = settlementAmount,
                        Currency = "USD", // TODO: Get from trip currency
                        Status = SettlementStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    });

                    memberBalances[creditor.Key] -= settlementAmount;
                    remainingDebt -= settlementAmount;
                }
            }

            // Save settlements to database
            _context.Settlements.AddRange(settlements);
            await _context.SaveChangesAsync();

            return settlements.Select(MapToSettlementResponse);
        }

        public async Task<SettlementResponse> MarkSettlementCompletedAsync(Guid settlementId)
        {
            var settlement = await _context.Settlements.FindAsync(settlementId);
            if (settlement == null)
                throw new ArgumentException("Settlement not found");

            settlement.Status = SettlementStatus.Completed;
            settlement.SettledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToSettlementResponse(settlement);
        }

        public async Task<IEnumerable<SettlementResponse>> GetTripSettlementsAsync(Guid tripId)
        {
            var settlements = await _context.Settlements
                .Where(s => s.TripId == tripId)
                .ToListAsync();

            return settlements.Select(MapToSettlementResponse);
        }

        private static SettlementResponse MapToSettlementResponse(Settlement settlement)
        {
            return new SettlementResponse
            {
                Id = settlement.Id,
                TripId = settlement.TripId,
                FromMemberId = settlement.FromMemberId,
                FromMemberName = settlement.FromMemberName,
                ToMemberId = settlement.ToMemberId,
                ToMemberName = settlement.ToMemberName,
                Amount = settlement.Amount,
                Currency = settlement.Currency,
                Status = settlement.Status,
                CreatedAt = settlement.CreatedAt,
                SettledAt = settlement.SettledAt
            };
        }
    }
}