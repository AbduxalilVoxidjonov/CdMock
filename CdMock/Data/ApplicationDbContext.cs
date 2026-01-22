using CdMock.Models;
using CdMock.Models.Listening;
using CdMock.Models.Reading;
using CdMock.Models.Writing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CdMock.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mocks> Mocks { get; set; }
        public DbSet<ReadingText> ReadingTexts { get; set; }
        public DbSet<ListeningAudio> ListeningAudios { get; set; }
        public DbSet<WritingTask> WritingTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // readingText relationship
            modelBuilder.Entity<ReadingText>()
                .HasOne(rt => rt.Mocks)
                .WithMany(m => m.ReadingTexts)
                .HasForeignKey(rt => rt.MockId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(); // Foreign key majburiy

            // ListeningAudio relationship
            modelBuilder.Entity<ListeningAudio>()
                .HasOne(la => la.Mocks)
                .WithMany(m => m.ListeningAudios)
                .HasForeignKey(la => la.MockId)
                .OnDelete(DeleteBehavior.Cascade);

            // WritingTask relationship
            modelBuilder.Entity<WritingTask>()
                .HasOne(wt => wt.Mocks)
                .WithMany(m => m.WritingTasks)
                .HasForeignKey(wt => wt.MockId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}