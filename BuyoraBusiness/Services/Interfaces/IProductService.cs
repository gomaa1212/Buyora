using BuyoraModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IProductService
    {
        public Task<Product> GetProductByIdAsync(int Id);
        public Task<Product> GetProductByIdWithIncludeAsync(int Id);
        public Task<(List<Product> Items,int TotalCount)> GetProductsListAsync(int page,int pageSize,string? search = null);
        public Task<string> AddProductAsync(Product product,List<IFormFile> files, IFormFile mainFile);
        public Task<string> DeleteProductAsync(int Id);
        public Task<string> UpdateProductAsync(Product product, List<IFormFile>? files, IFormFile? mainFile);
    }
}
