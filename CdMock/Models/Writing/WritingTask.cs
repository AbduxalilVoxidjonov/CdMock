using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CdMock.Models.Writing
{
    [Table("WritingTasks")]
    public class WritingTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Sarlavha kiritilishi shart")]
        [StringLength(200, ErrorMessage = "Sarlavha 200 belgidan oshmasligi kerak")]
        [Display(Name = "Task Sarlavha")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Task Turi")]
        [Required(ErrorMessage = "Task turini tanlang")]
        public TaskType TaskType { get; set; } // Task1 yoki Task2

        // Task 1 uchun rasm
        [Display(Name = "Task 1 Rasm")]
        public string? Task1ImagePath { get; set; }

        [Display(Name = "Task 1 Rasm Nomi")]
        public string? Task1ImageName { get; set; }

        // Task 2 uchun rasm
        [Display(Name = "Task 2 Rasm")]
        public string? Task2ImagePath { get; set; }

        [Display(Name = "Task 2 Rasm Nomi")]
        public string? Task2ImageName { get; set; }

        [Display(Name = "Task 1 Tavsif")]
        [StringLength(1000)]
        public string? Task1Description { get; set; }

        [Display(Name = "Task 2 Tavsif")]
        [StringLength(1000)]
        public string? Task2Description { get; set; }

        [Display(Name = "Task 1 Ko'rsatma")]
        [StringLength(500)]
        public string? Task1Instructions { get; set; }

        [Display(Name = "Task 2 Ko'rsatma")]
        [StringLength(500)]
        public string? Task2Instructions { get; set; }

        [Display(Name = "Vaqt cheklovi (daqiqalarda)")]
        public int? TimeLimit { get; set; } // Daqiqalarda

        [DataType(DataType.DateTime)]
        [Display(Name = "Yaratilgan Vaqt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key
        [Required(ErrorMessage = "Mock tanlash shart")]
        [Display(Name = "Mock Exam")]
        public int MockId { get; set; }

        // Navigation Property
        [ForeignKey("MockId")]
        public Mocks? Mocks { get; set; }
    }

    public enum TaskType
    {
        [Display(Name = "Task 1")]
        Task1 = 1,

        [Display(Name = "Task 2")]
        Task2 = 2,

        [Display(Name = "Ikkala Task")]
        Both = 3
    }
}