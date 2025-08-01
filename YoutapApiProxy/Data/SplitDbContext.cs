using Microsoft.EntityFrameworkCore;
using SplitManagement.Models;

namespace SplitManagement.Data
{
    public class SplitDbContext : DbContext
    {
        public SplitDbContext(DbContextOptions<SplitDbContext> options) : base(options) { }

        public DbSet<SpendingSplit> SpendingSplits { get; set; }
        public DbSet<SplitParticipant> SplitParticipants { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<SplitTag> SplitTags { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SpendingSplit configuration
            modelBuilder.Entity<SpendingSplit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SplitAmount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.PayerId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PayerName).HasMaxLength(200);
                
                entity.HasMany(e => e.Participants)
                    .WithOne(p => p.SpendingSplit)
                    .HasForeignKey(p => p.SpendingSplitId);
            });

            // SplitParticipant configuration
            modelBuilder.Entity<SplitParticipant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShareAmount).HasPrecision(18, 2);
                entity.Property(e => e.SharePercentage).HasPrecision(5, 2);
                entity.Property(e => e.MemberId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MemberName).HasMaxLength(200);
            });

            // Tag configuration
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Color).HasMaxLength(7);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // SplitTag configuration (Many-to-Many)
            modelBuilder.Entity<SplitTag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.SpendingSplit)
                    .WithMany(s => s.Tags)
                    .HasForeignKey(e => e.SpendingSplitId);
                entity.HasOne(e => e.Tag)
                    .WithMany(t => t.SplitTags)
                    .HasForeignKey(e => e.TagId);
            });

            // Settlement configuration
            modelBuilder.Entity<Settlement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.FromMemberId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ToMemberId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FromMemberName).HasMaxLength(200);
                entity.Property(e => e.ToMemberName).HasMaxLength(200);
            });
        }
    }
}