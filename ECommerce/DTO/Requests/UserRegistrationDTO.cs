using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO.Requests
{
    public class UserRegistrationDTO
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
