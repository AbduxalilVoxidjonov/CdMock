using CdMock.Models.Mock;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CdMock.Models;

namespace CdMock.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet'lar
        public DbSet<Mocks> Mocks { get; set; }
        public DbSet<Reading> Readings { get; set; }
        public DbSet<ReadingQuestion> ReadingQuestions { get; set; }
        public DbSet<Listening> Listenings { get; set; }
        public DbSet<ListeningQuestion> ListeningQuestions { get; set; }
        public DbSet<Writing> Writings { get; set; }
        public DbSet<UserMockResult> UserMockResults { get; set; }
        public DbSet<UserReadingAnswer> UserReadingAnswers { get; set; }
        public DbSet<UserListeningAnswer> UserListeningAnswers { get; set; }
        public DbSet<UserWritingAnswer> UserWritingAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Mock -> Reading (One-to-Many)
            builder.Entity<Reading>()
                .HasOne(r => r.Mock)
                .WithMany(m => m.Readings)
                .HasForeignKey(r => r.MockId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mock -> Listening (One-to-Many)
            builder.Entity<Listening>()
                .HasOne(l => l.Mock)
                .WithMany(m => m.Listenings)
                .HasForeignKey(l => l.MockId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mock -> Writing (One-to-Many)
            builder.Entity<Writing>()
                .HasOne(w => w.Mock)
                .WithMany(m => m.Writings)
                .HasForeignKey(w => w.MockId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reading -> ReadingQuestion (One-to-Many)
            builder.Entity<ReadingQuestion>()
                .HasOne(rq => rq.Reading)
                .WithMany(r => r.Questions)
                .HasForeignKey(rq => rq.ReadingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Listening -> ListeningQuestion (One-to-Many)
            builder.Entity<ListeningQuestion>()
                .HasOne(lq => lq.Listening)
                .WithMany(l => l.Questions)
                .HasForeignKey(lq => lq.ListeningId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> UserMockResult (One-to-Many)
            builder.Entity<UserMockResult>()
                .HasOne(umr => umr.User)
                .WithMany()
                .HasForeignKey(umr => umr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mock -> UserMockResult (One-to-Many)
            builder.Entity<UserMockResult>()
                .HasOne(umr => umr.Mock)
                .WithMany(m => m.UserMockResults)
                .HasForeignKey(umr => umr.MockId)
                .OnDelete(DeleteBehavior.NoAction); // SQL Server cascade path muammosini hal qiladi

            // UserMockResult -> UserReadingAnswer (One-to-Many)
            builder.Entity<UserReadingAnswer>()
                .HasOne(ura => ura.UserMockResult)
                .WithMany(umr => umr.ReadingAnswers)
                .HasForeignKey(ura => ura.UserMockResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // ReadingQuestion -> UserReadingAnswer (One-to-Many)
            builder.Entity<UserReadingAnswer>()
                .HasOne(ura => ura.ReadingQuestion)
                .WithMany()
                .HasForeignKey(ura => ura.ReadingQuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // UserMockResult -> UserListeningAnswer (One-to-Many)
            builder.Entity<UserListeningAnswer>()
                .HasOne(ula => ula.UserMockResult)
                .WithMany(umr => umr.ListeningAnswers)
                .HasForeignKey(ula => ula.UserMockResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // ListeningQuestion -> UserListeningAnswer (One-to-Many)
            builder.Entity<UserListeningAnswer>()
                .HasOne(ula => ula.ListeningQuestion)
                .WithMany()
                .HasForeignKey(ula => ula.ListeningQuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // UserMockResult -> UserWritingAnswer (One-to-Many)
            builder.Entity<UserWritingAnswer>()
                .HasOne(uwa => uwa.UserMockResult)
                .WithMany(umr => umr.WritingAnswers)
                .HasForeignKey(uwa => uwa.UserMockResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Writing -> UserWritingAnswer (One-to-Many)
            builder.Entity<UserWritingAnswer>()
                .HasOne(uwa => uwa.Writing)
                .WithMany(w => w.UserAnswers)
                .HasForeignKey(uwa => uwa.WritingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}