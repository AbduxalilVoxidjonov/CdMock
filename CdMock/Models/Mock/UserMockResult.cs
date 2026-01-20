using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class UserMockResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        [Required]
        public int MockId { get; set; }
        public Mocks Mock { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Ballar
        public int ReadingScore { get; set; } = 0;
        public int ListeningScore { get; set; } = 0;
        public int WritingScore { get; set; } = 0;

        public int TotalScore { get; set; } = 0;

        // Navigation Properties
        public ICollection<UserReadingAnswer> ReadingAnswers { get; set; }
        public ICollection<UserListeningAnswer> ListeningAnswers { get; set; }
        public ICollection<UserWritingAnswer> WritingAnswers { get; set; }
    }
}
