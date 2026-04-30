using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.DTOs.Category;

namespace TaskManagementSystem.Controllers{
    public class CategoryController : Controller{
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _categoryService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _categoryService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
    }
}