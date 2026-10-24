using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Buyora.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService,IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetCategoriesListAsync();
            var displays = _mapper.Map<List<GetCategoriesListViewModel>>(categories);
            return View(displays);
        }
        
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategoryViewModel model)
        {
            if(model is null)
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var category = _mapper.Map<Category>(model);
            var res =await _categoryService.AddCategoryAsync(category);
            if(res== "Succeeded")
            {
                TempData["SuccessMessage"] = "Category Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> IsCategoryNameEnExist(string NameEn)
        {
            var res = await _categoryService.IsCategoryNameEnExistAsync(NameEn);
            return Json(!res);
        }
        [HttpPost]
        public async Task<IActionResult> IsCategoryNameArExist(string NameAr)
        {
            var res = await _categoryService.IsCategoryNameArExistAsync(NameAr);
            return Json(!res);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }
            var category =await _categoryService.GetCategoryByIdAsync((int)id);
            if(category is null)
            {
                return NotFound();
            }
            var categoryModel = _mapper.Map<EditCategoryViewModel>(category);
            return View(categoryModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if(model is null)
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var category = _mapper.Map<Category>(model);
            var res = await _categoryService.UpdateCategoryAsync(category);
            if(res == "Succeeded")
            {
                TempData["SuccessMessage"] = "Category Edited Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> IsCategoryNameEnExistForEdit(string NameEn,int id)
        {
            var res = await _categoryService.IsCategoryNameEnExistForUpdateAsync(NameEn, id);
            return Json(!res);
        }
        [HttpPost]
        public async Task<IActionResult> IsCategoryNameArExistForEdit(string NameAr, int id)
        {
            var res = await _categoryService.IsCategoryNameArExistForUpdateAsync(NameAr, id);
            return Json(!res);
        }
        public async Task<IActionResult> Delete(int?id)
        {
            if(id==null ||id==0)
            {
                return NotFound();
            }
            var category = await _categoryService.GetCategoryByIdAsync((int)id);
            if(category == null)
            {
                return NotFound();
            }
            var categoryModel = _mapper.Map<GetCategoryByIdViewModel>(category);
            return View(categoryModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(GetCategoryByIdViewModel model)
        {
            if(model == null)
            {
                return NotFound();
            }
            var res =await _categoryService.DeleteCategoryAsync(model.Id);
            if (res == "Succeeded")
            {
                TempData["SuccessMessage"] = "Category Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
