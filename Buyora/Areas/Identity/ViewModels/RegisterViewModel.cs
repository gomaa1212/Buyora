using System.ComponentModel.DataAnnotations;

namespace Buyora.Areas.Identity.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Password and confirm password doesn't match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
