using Microsoft.AspNetCore.Mvc;
using BuyoraBusiness.Services.Interfaces;
using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Product;
using Microsoft.AspNetCore.Mvc.Rendering;
using Buyora.Helper;
using BuyoraModels;
namespace Buyora.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService,IMapper mapper,ICategoryService categoryService)
        {
            _productService = productService;
            _mapper = mapper;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsListAsync();
            var productsDisplay = _mapper.Map<List<GetProductsListViewModel>>(products);
            return View(productsDisplay);
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
    }
}
