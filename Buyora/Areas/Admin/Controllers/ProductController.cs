using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using Buyora.Areas.Admin.ViewModels.Product;
using Buyora.Helper;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Buyora.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly int pageSize = 4;
        public ProductController(IProductService productService,IMapper mapper,ICategoryService categoryService)
        {
            _productService = productService;
            _mapper = mapper;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index(string? search = null,string? categoryName = null,int page = 1)
        {
            var categories = await _categoryService.GetCategoriesListAsync();
            var products = await _productService.GetProductsListAsync(page, pageSize,categoryName,search);
            var model = new PageResult<GetProductsListViewModel>()
            {
                Items = _mapper.Map<List<GetProductsListViewModel>>(products.Items),
                TotalCount = products.TotalCount,
                Page=page,
                PageSize= pageSize,
                CategoryName= categoryName,
                Search = search
            };
            ViewBag.Categories = _mapper.Map<List<GetCategoriesListViewModel>>(categories);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DisplayProductsPartial", model);
            }
            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }
            var product = _mapper.Map<Product>(model);
            var res =await _productService.AddProductAsync(product,model.Files,model.MainFile);
            if(res == "Succeeded")
            {
                TempData["SuccessMessage"] = "Product added Successfully";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Failed to create product");
            await LoadCategoriesAsync();
            return View(model);
        }
        private async Task LoadCategoriesAsync(int? selectedCategoryId = null)
        {            
            ViewBag.Categories = new SelectList(
                await _categoryService.GetCategoriesListAsync(),
                "Id",
                CultureHelper.IsEnglish() ? "NameEn" : "NameAr" ,
                selectedCategoryId);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdWithIncludeAsync(id);
            if(product ==null)
            {
                return NotFound();
            }
            var productDisplay = _mapper.Map<GetProductByIdViewModel>(product);
            return View(productDisplay);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var product = await _productService.GetProductByIdWithIncludeAsync(Id);
            if(product == null)
            {
                return NotFound();
            }
            await LoadCategoriesAsync(product.CategoryId);
            var productModel = _mapper.Map<EditProductViewModel>(product);
            return View(productModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            ModelState.Remove("MainFile");
            if(!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model.CategoryId);
                return View(model);
            }
            var product = await _productService.GetProductByIdWithIncludeAsync(model.Id);
            if(product==null)
            {
                return NotFound();
            }
            _mapper.Map(model, product);
            var res = await _productService.UpdateProductAsync(product, model.Files, model.MainFile);
            if (res == "Succeeded")
            {
                TempData["SuccessMessage"] = "Product edited Successfully";
                return RedirectToAction(nameof(Index));
            }
            _mapper.Map(product, model);
            await LoadCategoriesAsync(model.CategoryId);
            ModelState.AddModelError(string.Empty, "Failed to update product");
            return View(model);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var product = await _productService.GetProductByIdWithIncludeAsync(Id);
            if(product == null)
            {
                return NotFound();
            }
            var productDisplay = _mapper.Map<GetProductByIdViewModel>(product);
            return View(productDisplay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(GetProductByIdViewModel model)
        {
            if(model == null)
            {
                return NotFound();
            }
            var res = await _productService.DeleteProductAsync(model.Id);
            if (res == "Succeeded")
            {
                TempData["SuccessMessage"] = "Product deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Failed to update product");
            return View(model);
        }
        public async Task<IActionResult> Search(string search,int page)
        {
            var products = await _productService.GetProductsListAsync(page,2,search);
            var productsDisplay = _mapper.Map<List<GetProductsListViewModel>>(products);
            return PartialView("_DisplayProductsPartial", productsDisplay);
        }
    }
}
