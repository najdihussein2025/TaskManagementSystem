using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.DTOs.Category;

namespace TaskManagementSystem.Controllers{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller{
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;

        public CategoryController(AppDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        private async Task SetSidebarCountsAsync()
        {
            ViewBag.SidebarUserCount = await _context.Users.CountAsync();
            ViewBag.SidebarTaskCount = await _context.Tasks.CountAsync();
            ViewBag.SidebarCategoryCount = await _context.Categories.CountAsync();
        }

        [HttpGet("Category")]
        [HttpGet("/Admin/Categories")]
        public async Task<IActionResult> Index()
        {
            await SetSidebarCountsAsync();
            var categories = await _categoryService.GetAllAsync();
            var mostUsed = categories.OrderByDescending(c => c.TaskCount).FirstOrDefault();
            var leastUsed = categories
                .Where(c => c.TaskCount > 0)
                .OrderBy(c => c.TaskCount)
                .FirstOrDefault();

            ViewBag.MostUsedCategory = mostUsed?.Name ?? "-";
            ViewBag.LeastUsedCategory = leastUsed?.Name ?? "-";
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
            await SetSidebarCountsAsync();
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                var mostUsed = categories.OrderByDescending(c => c.TaskCount).FirstOrDefault();
                var leastUsed = categories
                    .Where(c => c.TaskCount > 0)
                    .OrderBy(c => c.TaskCount)
                    .FirstOrDefault();
                ViewBag.MostUsedCategory = mostUsed?.Name ?? "-";
                ViewBag.LeastUsedCategory = leastUsed?.Name ?? "-";
                return View("~/Views/Admin/Categories.cshtml", categories);
            }

            await _categoryService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Category/Edit")]
        [HttpPost("/Admin/UpdateCategory")]
        public async Task<IActionResult> Edit(UpdateCategoryDto dto)
        {
            await SetSidebarCountsAsync();
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                var mostUsed = categories.OrderByDescending(c => c.TaskCount).FirstOrDefault();
                var leastUsed = categories
                    .Where(c => c.TaskCount > 0)
                    .OrderBy(c => c.TaskCount)
                    .FirstOrDefault();
                ViewBag.MostUsedCategory = mostUsed?.Name ?? "-";
                ViewBag.LeastUsedCategory = leastUsed?.Name ?? "-";
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