using System.ComponentModel.DataAnnotations;

namespace NetCoreIntermediate.Models
{
    public class TemporaryUser
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Password { get; set; }

        [Key]
        [Required]
        public required string Email { get; set; }
    }
}
