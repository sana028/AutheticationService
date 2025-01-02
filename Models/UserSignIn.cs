using System.ComponentModel.DataAnnotations;

namespace NetCoreIntermediate.Models
{
    public class UserSignIn
    {
        [Key]
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
