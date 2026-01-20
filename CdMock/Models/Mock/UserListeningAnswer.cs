using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class UserListeningAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserMockResultId { get; set; }
        public UserMockResult UserMockResult { get; set; }

        [Required]
        public int ListeningQuestionId { get; set; }
        public ListeningQuestion ListeningQuestion { get; set; }

        [Required]
        public string UserAnswer { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.Now;
    }
}
