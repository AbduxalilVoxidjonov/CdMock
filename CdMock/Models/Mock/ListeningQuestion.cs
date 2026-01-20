using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class ListeningQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListeningId { get; set; }
        public Listening Listening { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }

        public int OrderNumber { get; set; }

        public int Points { get; set; } = 1;
    }
}
