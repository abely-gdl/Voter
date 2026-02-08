using Microsoft.EntityFrameworkCore;
using VoterAPI.Models;

namespace VoterAPI.Data;

public class VoterDbContext : DbContext
{
    public VoterDbContext(DbContextOptions<VoterDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Suggestion> Suggestions { get; set; }
    public DbSet<Vote> Votes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>();
        });

        // Board configuration
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).HasMaxLength(200).IsRequired();
            entity.Property(b => b.Description).HasMaxLength(1000).IsRequired();
            entity.Property(b => b.VotingType).HasConversion<string>();

            entity.HasOne(b => b.CreatedBy)
                  .WithMany(u => u.Boards)
                  .HasForeignKey(b => b.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Suggestion configuration
        modelBuilder.Entity<Suggestion>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Text).HasMaxLength(500).IsRequired();
            entity.Property(s => s.Status).HasConversion<string>();

            entity.HasOne(s => s.Board)
                  .WithMany(b => b.Suggestions)
                  .HasForeignKey(s => s.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.SubmittedBy)
                  .WithMany(u => u.Suggestions)
                  .HasForeignKey(s => s.SubmittedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Vote configuration
        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(v => v.Id);

            // Ensure a user can only vote once per suggestion
            entity.HasIndex(v => new { v.SuggestionId, v.UserId }).IsUnique();

            entity.HasOne(v => v.Suggestion)
                  .WithMany(s => s.Votes)
                  .HasForeignKey(v => v.SuggestionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.User)
                  .WithMany(u => u.Votes)
                  .HasForeignKey(v => v.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed default admin user (password: admin123)
        // BCrypt hash for "admin123"
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "$2a$11$cqYR9g2uzVdlt0j0txokSeLbACCohBswHulHjeEDpLcff086G.NOK",
            Role = UserRole.Admin,
            CreatedDate = seedDate
        });

        // Seed sample boards
        modelBuilder.Entity<Board>().HasData(
            new Board
            {
                Id = 1,
                Title = "Product Features",
                Description = "Vote on which features you'd like to see in our next release",
                CreatedByUserId = 1,
                CreatedDate = seedDate,
                IsSuggestionsOpen = true,
                IsVotingOpen = true,
                RequireApproval = false,
                VotingType = VotingType.Multiple,
                MaxVotes = 3,
                IsClosed = false
            },
            new Board
            {
                Id = 2,
                Title = "Office Improvements",
                Description = "Suggest and vote on office improvement ideas",
                CreatedByUserId = 1,
                CreatedDate = seedDate,
                IsSuggestionsOpen = true,
                IsVotingOpen = true,
                RequireApproval = true,
                VotingType = VotingType.Single,
                MaxVotes = null,
                IsClosed = false
            },
            new Board
            {
                Id = 3,
                Title = "Team Event Ideas",
                Description = "What should we do for our next team event?",
                CreatedByUserId = 1,
                CreatedDate = seedDate,
                IsSuggestionsOpen = false,
                IsVotingOpen = true,
                RequireApproval = false,
                VotingType = VotingType.Single,
                MaxVotes = null,
                IsClosed = false
            }
        );
    }
}
