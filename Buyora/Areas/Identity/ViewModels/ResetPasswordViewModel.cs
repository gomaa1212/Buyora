using System.ComponentModel.DataAnnotations;

namespace Buyora.Areas.Identity.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirm password doesn't match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
