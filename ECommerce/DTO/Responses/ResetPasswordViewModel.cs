using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO.Responses
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
