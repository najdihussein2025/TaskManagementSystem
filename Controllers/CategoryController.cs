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

        [HttpGet("Category")]
        [HttpGet("/Admin/Categories")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View("~/Views/Admin/Categories.cshtml", categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Category/Create")]
        [HttpPost("/Admin/CreateCategory")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                return View("~/Views/Admin/Categories.cshtml", categories);
            }

            await _categoryService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Category/Edit")]
        [HttpPost("/Admin/UpdateCategory")]
        public async Task<IActionResult> Edit(UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                return View("~/Views/Admin/Categories.cshtml", categories);
            }

            await _categoryService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Category/Delete")]
        [HttpPost("/Admin/DeleteCategory")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}