using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CdMock.Models.Reading
{
    [Table("ReadingTexts")]
    public class ReadingText
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TextId { get; set; }

        [Required(ErrorMessage = "Sarlavha kiritilishi shart")]
        [StringLength(200, ErrorMessage = "Sarlavha 200 belgidan oshmasligi kerak")]
        [Display(Name = "Sarlavha")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Matn kiritilishi shart")]
        [Display(Name = "Reading Text")]
        public string Text { get; set; } = string.Empty;

        // Foreign Key
        [Required(ErrorMessage = "Mock tanlash shart")]
        [Display(Name = "Mock Exam")]
        public int MockId { get; set; }

        // Navigation Property - NULL qabul qilishi kerak!
        [ForeignKey("MockId")]
        public Mocks? Mocks { get; set; } // ? belgisi qo'shing!
    }
}