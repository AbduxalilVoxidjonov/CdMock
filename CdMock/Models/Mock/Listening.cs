using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class Listening
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MockId { get; set; }
        public Mocks Mock { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // Part 1, Part 2

        [Required]
        public string AudioUrl { get; set; } // Audio fayl manzili

        public int OrderNumber { get; set; }

        [StringLength(2000)]
        public string? Transcript { get; set; } // Audio matni (optional)

        // Navigation Property
        public ICollection<ListeningQuestion> Questions { get; set; }
    }
}
