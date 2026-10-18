
using BuyoraBusiness.Services.Interfaces;
using BuyoraDataAccess.UnitOfWorks;
using BuyoraModels;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace BuyoraBusiness.Services.Implementations
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
                if(category is null)
                {
                    throw new ArgumentException("Failed");
                }
                await _unitOfWork.Repository<Category>().AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            
        }

        public async Task<string> DeleteCategoryAsync(int Id)
        {
                var category = await GetCategoryByIdAsync(Id);
                if(category is null)
                {
                     throw new ArgumentException("Failed");
                }
                 _unitOfWork.Repository<Category>().Delete(category);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
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
            return await _unitOfWork.Repository<Category>().GetAsQuerable().AnyAsync(x => x.NameAr.ToLower() == NameEn.ToLower());
        }

        public async Task<bool> IsCategoryNameEnExistForUpdateAsync(string NameEn,int Id)
        {
            return await _unitOfWork.Repository<Category>().GetAsQuerable().AnyAsync(x => x.NameEn.ToLower() == NameEn.ToLower() && x.Id != Id);
        }

        public async Task<string> UpdateCategoryAsync(Category category)
        {
                if(category is null)
                {
                    throw new ArgumentException("Failed");
                }  
            _unitOfWork.Repository<Category>().Update(category);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
        }
    }
}
