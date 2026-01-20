using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class ReadingQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReadingId { get; set; }
        public Reading Reading { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; } // Multiple Choice, True/False, Fill in Blank

        [Required]
        public string CorrectAnswer { get; set; }

        public string? OptionA { get; set; } // Multiple choice uchun
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }

        public int OrderNumber { get; set; } // Savol tartib raqami

        public int Points { get; set; } = 1; // Har bir savol uchun ball
    }
}
