using BuyoraBusiness.Services.Interfaces;
using BuyoraDataAccess.UnitOfWorks;
using BuyoraModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BuyoraBusiness.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IFileService fileService,ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<Product> GetProductByIdAsync(int Id)
        {
            return await _unitOfWork.Repository<Product>().GeByIdAsync(Id);
        }

        public async Task<Product> GetProductByIdWithIncludeAsync(int Id)
        {
            return await _unitOfWork.Repository<Product>().GetAsQuerable().Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<(List<Product> Items, int TotalCount)> GetProductsListAsync(int page,int pageSize, string? categoryName = null, string? search = null)
        {
            IQueryable<Product> products = _unitOfWork.Repository<Product>().GetAsQuerable().AsNoTracking().Include(x => x.ProductImages);
            var totalCount = 0;
            if(!string.IsNullOrWhiteSpace(categoryName))
            {
    
                products = products.Include(x=>x.Category).Where(x => x.Category.NameAr == categoryName || x.Category.NameEn == categoryName);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                      .Where(x =>
                          x.NameAr.Contains(search) ||
                          x.NameEn.Contains(search) ||
                          (x.DescriptionAr != null && x.DescriptionAr.Contains(search)) ||
                          (x.DescriptionEn != null && x.DescriptionEn.Contains(search)));
            }
            
            totalCount =await products.CountAsync();
            var items =await products.OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);

        }
        
        public async Task<string> AddProductAsync(Product product, List<IFormFile> files , IFormFile mainFile)
        {     
            var uploadedUrls = new List<string>();

            using (var trans = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (product == null)
                        throw new ArgumentException("Failed");

                    if (mainFile == null)
                        throw new ArgumentException("Main Image is required");

                    await _unitOfWork.Repository<Product>().AddAsync(product);
                    await _unitOfWork.SaveChangesAsync();

                    var productImages = new List<ProductImage>();

                    if(files !=null && files.Any())
                    {
                        foreach(var file in files)
                        {
                            var url = await _fileService.UploadFileAsync(file, "Images/Products");
                            uploadedUrls.Add(url);

                            var productImage = new ProductImage()
                            {
                                Url = url,
                                IsMain = false,
                                ProductId = product.Id
                            };
                            productImages.Add(productImage);
                        }
                    }
                    var mainUrl = await _fileService.UploadFileAsync(mainFile, "Images/Products");
                    uploadedUrls.Add(mainUrl);

                    productImages.Add(new ProductImage()
                    {
                        Url = mainUrl,
                        IsMain = true,
                        ProductId = product.Id
                    });
                    await _unitOfWork.Repository<ProductImage>().AddRangeAsync(productImages);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    return "Succeeded";
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    foreach (var url in uploadedUrls)
                    {
                        _fileService.DeleteFile(url);
                    }
                    _logger.LogError(ex, "Failed to add product");
                    return "Failed to add product";
                }
            }
        }

        public async Task<string> DeleteProductAsync(int Id)
        {
            using var trans = await _unitOfWork.BeginTransactionAsync();
            var product = await GetProductByIdWithIncludeAsync(Id);
            if (product == null)
            {
                throw new ArgumentException("Failed");
            }

            var oldUrls = product.ProductImages.Select(x => x.Url).ToList();
            try
            {
                _unitOfWork.Repository<Product>().Delete(product);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product");
                await _unitOfWork.RollbackTransactionAsync();
                return "Failed";
            }
            foreach (var url in oldUrls)
            {
                if (!_fileService.DeleteFile(url))
                {
                    _logger.LogWarning("Could not delete old file {Url}", url);
                }
            }
            return "Succeeded";
        }
             
        
        public async Task<string> UpdateProductAsync(Product product,List<IFormFile>?files,IFormFile? mainFile)
        {
            using (var trans = await _unitOfWork.BeginTransactionAsync())
            {
                var uploadedUrls = new List<string>();
                try
                {
                    var oldUrls = new List<string>();

                    if (product == null)
                    {
                        throw new ArgumentException("Failed");
                    }


                    var productImages = new List<ProductImage>();

                    if(files!=null && files.Any())
                    {
                        foreach(var image in product.ProductImages.Where(x=>x.IsMain==false))
                        {
                            oldUrls.Add(image.Url);

                            _unitOfWork.Repository<ProductImage>().Delete(image);
                        }
                
                        foreach(var file in files)
                        {
                            var url = await _fileService.UploadFileAsync(file, "Images/Products");
                            uploadedUrls.Add(url);

                            productImages.Add(new ProductImage()
                            {
                                Url = url,
                                IsMain = false,
                                ProductId = product.Id
                            });
                        }
                        await _unitOfWork.Repository<ProductImage>().AddRangeAsync(productImages);
                    }
                    if(mainFile!=null)
                    {
                        var mainImage = product.ProductImages.FirstOrDefault(x => x.IsMain == true);
                        oldUrls.Add(mainImage.Url);

                        var url = await _fileService.UploadFileAsync(mainFile, "Images/Products");
                        uploadedUrls.Add(url);

                        mainImage.Url = url;
                    }
                    
                    _unitOfWork.Repository<Product>().Update(product);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    foreach(var url in oldUrls)
                    {
                        if(!_fileService.DeleteFile(url))
                        {
                            _logger.LogWarning("Could not delete old file {Url}", url);
                        }
                    }
                    return "Succeeded";
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    foreach (var url in uploadedUrls)
                    {
                        _fileService.DeleteFile(url);
                    }
                       
                    _logger.LogError(ex, "Failed to update product");
                    return "Failed";
                }
            }
        }
        

    }
}
