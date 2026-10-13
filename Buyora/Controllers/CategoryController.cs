using AutoMapper;
using Buyora.Models.Services.Interfaces;
using Buyora.Models.ViewModel.Category;
using Microsoft.AspNetCore.Mvc;

namespace Buyora.Controllers
{
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
    }
}
