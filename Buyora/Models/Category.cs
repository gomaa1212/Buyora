using System.ComponentModel.DataAnnotations;

namespace Buyora.Models
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string NameEn { get; set; } = string.Empty;
        [StringLength(100)]
        public string NameAr { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public int? DisplayOrder { get; set; }
    }
}
