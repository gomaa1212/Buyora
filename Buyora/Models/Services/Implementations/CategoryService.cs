using Buyora.Models.Services.Interfaces;
using Buyora.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace Buyora.Models.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> AddCategoryAsync(Category category)
        {
            try
            {
                if(category is null)
                {
                    return "Failed";
                }
                await _unitOfWork.Repository<Category>().AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                return "Failed";
            }
        }

        public async Task<string> DeleteCategoryAsync(int Id)
        {
            try
            {
                var category = await GetCategoryByIdAsync(Id);
                if(category is null)
                {
                    return "Failed";
                }
                 _unitOfWork.Repository<Category>().Delete(category);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                return "Failed";
            }
        }

        public async Task<List<Category>> GetCategoriesListAsync()
        {
            return await _unitOfWork.Repository<Category>().GetAsListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int Id)
        {
            var category = await _unitOfWork.Repository<Category>().GeByIdAsync(Id);
            return category;
        }

        public async Task<bool> IsCategoryNameArExistAsync(string NameAr)
        {
            return await _unitOfWork.Repository<Category>().GetAsQuerable().AnyAsync(x => x.NameAr == NameAr);
        }

        public async Task<bool> IsCategoryNameArExistForUpdateAsync(string NameAr , int Id)
        {
            return await _unitOfWork.Repository<Category>().GetAsQuerable().AnyAsync(x => x.NameAr == NameAr && x.Id != Id);
        }

        public async Task<bool> IsCategoryNameEnExistAsync(string NameEn)
        {
            return await _unitOfWork.Repository<Category>().GetAsQuerable().AnyAsync(x => x.NameEn == NameEn);
        }

        public async Task<bool> IsCategoryNameEnExistForUpdateAsync(string NameEn,int Id)
        {
            return await _unitOfWork.Repository<Category>().GetAsQuerable().AnyAsync(x => x.NameEn == NameEn && x.Id != Id);
        }

        public async Task<string> UpdateCategoryAsync(Category category)
        {
            try
            {
                var oldCategory = await GetCategoryByIdAsync(category.Id);
                if(oldCategory is null)
                {
                    return "Failed";
                }
                oldCategory.NameEn = category.NameEn;
                oldCategory.NameAr = category.NameAr;
                oldCategory.IsActive = category.IsActive;
                oldCategory.DisplayOrder = category.DisplayOrder;
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                return "Failed";
            }

        }
    }
}
