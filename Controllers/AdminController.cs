using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.DTOs.Category;
using TaskManagementSystem.DTOs.User;

namespace TaskManagementSystem.Controllers;

public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly ICategoryService _categoryService;

    public AdminController(IUserService userService, ICategoryService categoryService)
    {
        _userService = userService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Users(string? search, string? status)
    {
        var users = await _userService.GetAllAsync(search, status);
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        await _userService.CreateAsync(dto);
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
    {
        await _userService.UpdateAsync(dto);
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteAsync(id);
        return RedirectToAction(nameof(Users));
    }


    [HttpGet]
    public IActionResult Tasks()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Categories()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            return View(nameof(Categories), categories);
        }

        await _categoryService.CreateAsync(dto);
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            return View(nameof(Categories), categories);
        }

        await _categoryService.UpdateAsync(dto);
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteAsync(id);
        return RedirectToAction(nameof(Categories));
    }

    [HttpGet]
    public IActionResult Reports()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Settings()
    {
        return View();
    }
}
