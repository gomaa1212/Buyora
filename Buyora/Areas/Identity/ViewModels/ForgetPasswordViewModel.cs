using System.ComponentModel.DataAnnotations;

namespace Buyora.Areas.Identity.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
