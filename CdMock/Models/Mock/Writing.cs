using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class Writing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MockId { get; set; }
        public Mocks Mock { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // Task 1, Task 2

        [Required]
        public string TaskDescription { get; set; } // Topshiriq matni

        public WritingTaskType TaskType { get; set; } // Task 1, Task 2

        [Range(50, 500)]
        public int MinWords { get; set; } // Minimal so'zlar soni

        public int OrderNumber { get; set; }

        public int Points { get; set; } = 10; // Task 1 = 10, Task 2 = 20

        // Navigation Property
        public ICollection<UserWritingAnswer> UserAnswers { get; set; }
    }
}
