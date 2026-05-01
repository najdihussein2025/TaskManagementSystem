using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Dtos.Settings;
using TaskManagementSystem.DTOs.Task;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.DTOs.User;

namespace TaskManagementSystem.Controllers;

public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;
    private readonly ICategoryService _categoryService;
    private readonly IDashboardService _dashboardService;
    private readonly IReportsService _reportsService;
    private readonly ISettingsService _settingsService;

    public AdminController(
        IUserService userService,
        ITaskService taskService,
        ICategoryService categoryService,
        IDashboardService dashboardService,
        IReportsService reportsService,
        ISettingsService settingsService)
    {
        _userService = userService;
        _taskService = taskService;
        _categoryService = categoryService;
        _dashboardService = dashboardService;
        _reportsService = reportsService;
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var data = await _dashboardService.GetDashboardDataAsync();
        return View(data);
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
    public async Task<IActionResult> Tasks(
        string? search,
        string? status,
        string? priority,
        int? categoryId,
        int? userId)
    {
        var filter = new TaskFilterDto
        {
            Search = search,
            Status = status,
            Priority = priority,
            CategoryId = categoryId,
            UserId = userId
        };

        var tasks = await _taskService.FilterTasksAsync(filter);
        var users = await _userService.GetAllAsync();
        var categories = await _categoryService.GetAllAsync();

        ViewBag.Users = users;
        ViewBag.Categories = categories;
        ViewBag.Filter = filter;

        return View(tasks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTask(CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required fields.";
            return RedirectToAction(nameof(Tasks));
        }

        await _taskService.CreateTaskAsync(dto);
        TempData["Success"] = "Task created successfully.";
        return RedirectToAction(nameof(Tasks));
    }

    [HttpGet]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null) return NotFound();
        return Json(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTask(int id, UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid task data.";
            return RedirectToAction(nameof(Tasks));
        }

        var result = await _taskService.UpdateTaskAsync(id, dto);
        if (result == null)
        {
            TempData["Error"] = "Task not found.";
            return RedirectToAction(nameof(Tasks));
        }

        TempData["Success"] = "Task updated successfully.";
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var success = await _taskService.DeleteTaskAsync(id);
        TempData[success ? "Success" : "Error"] =
            success ? "Task deleted." : "Task not found.";

        return RedirectToAction(nameof(Tasks));
    }
    

    [HttpGet]
    public IActionResult Categories()
    {
        return RedirectToAction("Index", "Category");
    }

    [HttpGet]
    public async Task<IActionResult> Reports()
    {
        var data = await _reportsService.GetReportsDataAsync();
        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        // Get logged-in admin user ID from session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var profile = await _settingsService.GetAdminProfileAsync(userId.Value);
        if (profile == null) return RedirectToAction("Login", "Auth");

        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        ViewBag.ErrorMessage = TempData["ErrorMessage"];

        return View(profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(UpdateProfileDto dto)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var (success, message) = await _settingsService.UpdateProfileAsync(userId.Value, dto);

        if (success)
            TempData["SuccessMessage"] = message;
        else
            TempData["ErrorMessage"] = message;

        return RedirectToAction(nameof(Settings));
    }
}
