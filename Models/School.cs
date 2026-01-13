// School entity

using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class School
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public string Principal { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
