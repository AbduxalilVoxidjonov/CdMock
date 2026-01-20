using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class Reading
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MockId { get; set; }
        public Mocks Mock { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // Passage 1, Passage 2

        [Required]
        public string PassageText { get; set; } // O'qish uchun matn

        public int OrderNumber { get; set; } // Tartib raqami (1, 2, 3)

        // Navigation Property
        public ICollection<ReadingQuestion> Questions { get; set; }
    }
}
