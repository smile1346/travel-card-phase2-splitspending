using SplitManagement.DTOs;
using SplitManagement.Models;

namespace SplitManagement.Services
{
    public interface ISplitService
    {
        Task<SplitResponse> CreateSplitAsync(CreateSplitRequest request);
        Task<SplitResponse?> GetSplitByIdAsync(Guid id);
        Task<IEnumerable<SplitResponse>> GetSplitsByTripIdAsync(Guid tripId);
        Task<SplitResponse> UpdateSplitAsync(Guid id, CreateSplitRequest request);
        Task DeleteSplitAsync(Guid id);
        Task MarkParticipantPaidAsync(Guid participantId);
    }

    public interface ISettlementService
    {
        Task<IEnumerable<SettlementResponse>> CalculateSettlementsAsync(Guid tripId);
        Task<SettlementResponse> MarkSettlementCompletedAsync(Guid settlementId);
        Task<IEnumerable<SettlementResponse>> GetTripSettlementsAsync(Guid tripId);
    }
}