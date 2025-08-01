using SplitManagement.Models;

namespace SplitManagement.Repositories
{
    public interface ISplitRepository
    {
        Task<SpendingSplit?> GetByIdAsync(Guid id);
        Task<IEnumerable<SpendingSplit>> GetByTripIdAsync(Guid tripId);
        Task<SpendingSplit> CreateAsync(SpendingSplit split);
        Task<SpendingSplit> UpdateAsync(SpendingSplit split);
        Task DeleteAsync(Guid id);
        Task MarkParticipantPaidAsync(Guid participantId);
    }

    public interface ITagRepository
    {
        Task<Tag?> GetByNameAsync(string name);
        Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
        Task<Tag> CreateAsync(Tag tag);
    }
}