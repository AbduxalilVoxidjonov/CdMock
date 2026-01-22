using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CdMock.Models.Listening
{
    [Table("ListeningAudios")]
    public class ListeningAudio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AudioId { get; set; }

        [Required(ErrorMessage = "Sarlavha kiritilishi shart")]
        [StringLength(200, ErrorMessage = "Sarlavha 200 belgidan oshmasligi kerak")]
        [Display(Name = "Audio Sarlavha")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Tavsif")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Audio fayl yuklanishi shart")]
        [Display(Name = "Audio Fayl")]
        public string AudioFilePath { get; set; } = string.Empty; // Fayl yo'li

        [Display(Name = "Fayl Nomi")]
        public string FileName { get; set; } = string.Empty;

        [Display(Name = "Fayl Hajmi (bytes)")]
        public long FileSize { get; set; }

        [Display(Name = "Davomiyligi (soniyalarda)")]
        public int? Duration { get; set; } // Sekundlarda

        [DataType(DataType.DateTime)]
        [Display(Name = "Yuklangan Vaqt")]
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Foreign Key
        [Required(ErrorMessage = "Mock tanlash shart")]
        [Display(Name = "Mock Exam")]
        public int MockId { get; set; }

        // Navigation Property
        [ForeignKey("MockId")]
        public Mocks? Mocks { get; set; }
    }
}