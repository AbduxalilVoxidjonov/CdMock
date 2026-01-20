using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class UserReadingAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserMockResultId { get; set; }
        public UserMockResult UserMockResult { get; set; }

        [Required]
        public int ReadingQuestionId { get; set; }
        public ReadingQuestion ReadingQuestion { get; set; }

        [Required]
        public string UserAnswer { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.Now;
    }
}
