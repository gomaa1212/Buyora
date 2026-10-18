using BuyoraModels;
using Microsoft.AspNetCore.Http;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IProductService
    {
        public Task<Product> GetProductByIdAsync(int Id);
        public Task<Product> GetProductByIdWithIncludeAsync(int Id);
        public Task<List<Product>> GetProductsListAsync();
        public Task<string> AddProductAsync(Product product,List<IFormFile> files, IFormFile mainFile);
        public Task<string> DeleteProductAsync(int Id);
        public Task<string> UpdateProductAsync(Product product, List<IFormFile>? files, IFormFile? mainFile);
    }
}
