namespace Buyora.Models.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetCategoriesListAsync();
        public Task<Category> GetCategoryByIdAsync(int Id);
        public Task<string> AddCategoryAsync(Category category);
        public Task<string> DeleteCategoryAsync(int Id);
        public Task<string> UpdateCategoryAsync(Category category);
        public Task<bool> IsCategoryNameEnExistAsync(string NameEn);
        public Task<bool> IsCategoryNameArExistAsync(string NameAr);
        public Task<bool> IsCategoryNameEnExistForUpdateAsync(string NameEn,int Id);
        public Task<bool> IsCategoryNameArExistForUpdateAsync(string NameAr,int Id);
    }
}
