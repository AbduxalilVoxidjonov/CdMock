using System.ComponentModel.DataAnnotations;

namespace CdMock.Models.Mock
{
    public class Mocks
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // Mock Test 1, Mock Test 2

        [StringLength(1000)]
        public string Description { get; set; } // Test haqida ma'lumot

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true; // Test faolmi?

        [Range(1, 300)]
        public int TimeLimit { get; set; } // Umumiy vaqt (daqiqalarda)

        // Navigation Properties
        public ICollection<Reading> Readings { get; set; }
        public ICollection<Listening> Listenings { get; set; }
        public ICollection<Writing> Writings { get; set; }
        public ICollection<UserMockResult> UserMockResults { get; set; }
    }
}


