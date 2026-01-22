using CdMock.Models.Listening;
using CdMock.Models.Reading;
using CdMock.Models.Writing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CdMock.Models
{
    [Table("Mocks")]
    public class Mocks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nom kiritilishi shart")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Mock Nomi")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ta'rif berish majburiy")]
        [MaxLength(200)]
        [Display(Name = "Qisqacha Ta'rif")]
        public string Description { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Yaratilgan Vaqt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Aktivlik Holati")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties - NULL qabul qilishi mumkin
        public ICollection<ReadingText>? ReadingTexts { get; set; }
        public ICollection<ListeningAudio>? ListeningAudios { get; set; }
        public ICollection<WritingTask>? WritingTasks { get; set; }
    }
}