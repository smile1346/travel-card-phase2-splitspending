using Microsoft.EntityFrameworkCore;
using SplitManagement.Data;
using SplitManagement.Models;

namespace SplitManagement.Repositories
{
    public class SplitRepository : ISplitRepository
    {
        private readonly SplitDbContext _context;

        public SplitRepository(SplitDbContext context)
        {
            _context = context;
        }

        public async Task<SpendingSplit?> GetByIdAsync(Guid id)
        {
            return await _context.SpendingSplits
                .Include(s => s.Participants)
                .Include(s => s.Tags)
                    .ThenInclude(st => st.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SpendingSplit>> GetByTripIdAsync(Guid tripId)
        {
            return await _context.SpendingSplits
                .Include(s => s.Participants)
                .Include(s => s.Tags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.TripId == tripId)
                .ToListAsync();
        }

        public async Task<SpendingSplit> CreateAsync(SpendingSplit split)
        {
            split.Id = Guid.NewGuid();
            split.CreatedAt = DateTime.UtcNow;
            split.UpdatedAt = DateTime.UtcNow;

            foreach (var participant in split.Participants)
            {
                participant.Id = Guid.NewGuid();
            }

            _context.SpendingSplits.Add(split);
            await _context.SaveChangesAsync();
            return split;
        }

        public async Task<SpendingSplit> UpdateAsync(SpendingSplit split)
        {
            split.UpdatedAt = DateTime.UtcNow;
            _context.SpendingSplits.Update(split);
            await _context.SaveChangesAsync();
            return split;
        }

        public async Task DeleteAsync(Guid id)
        {
            var split = await _context.SpendingSplits.FindAsync(id);
            if (split != null)
            {
                _context.SpendingSplits.Remove(split);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkParticipantPaidAsync(Guid participantId)
        {
            var participant = await _context.SplitParticipants.FindAsync(participantId);
            if (participant != null)
            {
                participant.IsPaid = true;
                participant.PaidAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }

    public class TagRepository : ITagRepository
    {
        private readonly SplitDbContext _context;

        public TagRepository(SplitDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
        {
            var tags = new List<Tag>();
            
            foreach (var tagName in tagNames)
            {
                var existingTag = await GetByNameAsync(tagName);
                if (existingTag != null)
                {
                    tags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = tagName,
                        Color = GenerateRandomColor(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Tags.Add(newTag);
                    tags.Add(newTag);
                }
            }

            await _context.SaveChangesAsync();
            return tags;
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            tag.Id = Guid.NewGuid();
            tag.CreatedAt = DateTime.UtcNow;
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        private static string GenerateRandomColor()
        {
            var colors = new[] { "#FF6B6B", "#4ECDC4", "#45B7D1", "#96CEB4", "#FFEAA7", "#DDA0DD", "#98D8C8" };
            var random = new Random();
            return colors[random.Next(colors.Length)];
        }
    }
}