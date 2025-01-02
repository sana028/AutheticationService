using System.ComponentModel.DataAnnotations;

namespace NetCoreIntermediate.Models
{
    public class OtpInfos
    {
        [Required]
        public required string otp {  get; set; }

        [Key]
        [Required]
        public required string email { get; set; }
    }
}
