using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Buyora.Areas.Admin.ViewModels.Category
{
    public class AddCategoryViewModel : SharedCategoryViewModel
    {
        [Required]
        [Remote(action: "IsCategoryNameEnExist", controller: "Category", HttpMethod = "Post", ErrorMessage = "This name is already exist")]
        public string NameEn { get; set; }
        [Required]
        [Remote(action: "IsCategoryNameArExist", controller: "Category", HttpMethod = "Post", ErrorMessage = "This name is already exist")]
        public string NameAr { get; set; }
        
    }
}
