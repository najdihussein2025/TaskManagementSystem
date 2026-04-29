using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.DTOs.User;

namespace TaskManagementSystem.Controllers;

public class AdminController : Controller
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        await _userService.CreateAsync(dto);
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public IActionResult Tasks()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Categories()
    {
        return View();
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
