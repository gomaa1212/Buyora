using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Buyora.Areas.Admin.ViewModels.Category
{
    public class EditCategoryViewModel : SharedCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [Remote(action: "IsCategoryNameEnExistForEdit", controller: "Category", HttpMethod = "Post",AdditionalFields =nameof(Id), ErrorMessage = "This name is already exist")]
        public string NameEn { get; set; }
        [Required]
        [Remote(action: "IsCategoryNameArExistForEdit", controller: "Category", HttpMethod = "Post", AdditionalFields = nameof(Id), ErrorMessage = "This name is already exist")]
        public string NameAr { get; set; }
    }
}
