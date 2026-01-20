using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class UserWritingAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserMockResultId { get; set; }
        public UserMockResult UserMockResult { get; set; }

        [Required]
        public int WritingId { get; set; }
        public Writing Writing { get; set; }

        [Required]
        public string UserAnswer { get; set; } // User yozgan matn

        public int WordCount { get; set; }

        public int? Score { get; set; } // Admin tomonidan beriladigan ball

        public string? FeedBack { get; set; } // Admin fikri

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
