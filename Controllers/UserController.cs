using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.DTOs.Auth;
using TaskManagementSystem.DTOs.Task;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.Services;
using TaskStatus = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManagementSystem.Controllers;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly AppDbContext _context;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly ICategoryService _categoryService;
    private readonly IDashboardService _dashboardService;
    private readonly IAuthService _authService;

    public UserController(
        AppDbContext context,
        ITaskService taskService,
        IUserService userService,
        ICategoryService categoryService,
        IDashboardService dashboardService,
        IAuthService auth)
    {
        _context = context;
        _taskService = taskService;
        _userService = userService;
        _categoryService = categoryService;
        _dashboardService = dashboardService;
        _authService = auth;
    }

    private async Task SetSidebarCountsAsync()
    {
        ViewBag.SidebarUserCount = await _context.Users.CountAsync();
        ViewBag.SidebarTaskCount = await _context.Tasks.CountAsync();
        ViewBag.SidebarCategoryCount = await _context.Categories.CountAsync();
    }

    private int? GetCurrentUserId()
    {
        return User.GetUserId();
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);
        ViewBag.UserName = User.Identity?.Name ?? "User";
        return View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> MyTasks(string? search, string? status, string? priority, int? categoryId)
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var filter = new TaskFilterDto
        {
            Search = search,
            Status = status,
            Priority = priority,
            CategoryId = categoryId,
            UserId = userId.Value
        };

        var tasks = await _taskService.FilterTasksAsync(filter);
        var categories = await _categoryService.GetAllAsync();

        ViewBag.Categories = categories;
        ViewBag.Filter = filter;
        ViewBag.UserId = userId.Value;

        return View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> CreateTask()
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = categories;
        ViewBag.UserId = userId;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTask(CreateTaskDto dto)
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            TempData["Error"] = "Please fill in all required fields.";
            return View(dto);
        }

        if (dto.AssignedToUserId == null || dto.AssignedToUserId == 0)
        {
            dto.AssignedToUserId = userId.Value;
        }

        await _taskService.CreateTaskAsync(dto);
        TempData["Success"] = "Task created successfully!";
        return RedirectToAction(nameof(MyTasks));
    }

    [HttpGet]
    public async Task<IActionResult> EditTask(int id)
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            TempData["Error"] = "Task not found.";
            return RedirectToAction(nameof(MyTasks));
        }

        // Verify the task belongs to the current user
        if (task.AssignedToUserId != userId.Value && task.CreatedByUserId != userId.Value)
        {
            TempData["Error"] = "You don't have permission to edit this task.";
            return RedirectToAction(nameof(MyTasks));
        }

        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = categories;

        var updateDto = new UpdateTaskDto
        {
            Title = task.Title,
            Description = task.Description,
            AssignedToUserId = task.AssignedToUserId,
            CategoryId = task.CategoryId,
            Priority = task.Priority,
            DueDate = task.DueDate,
            Status = task.Status
        };

        ViewBag.TaskId = id;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTask(int id, UpdateTaskDto dto)
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.TaskId = id;
            TempData["Error"] = "Please correct the errors below.";
            return View(dto);
        }

        var result = await _taskService.UpdateTaskAsync(id, dto);
        if (result == null)
        {
            TempData["Error"] = "Task not found or you don't have permission.";
            return RedirectToAction(nameof(MyTasks));
        }

        TempData["Success"] = "Task updated successfully!";
        return RedirectToAction(nameof(MyTasks));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, string status)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null)
        {
            TempData["Error"] = "Task not found.";
            return RedirectToAction(nameof(MyTasks));
        }

        if (task.AssignedToUserId != userId.Value)
        {
            TempData["Error"] = "You don't have permission to update this task.";
            return RedirectToAction(nameof(MyTasks));
        }

        // Parse status string to enum
        if (Enum.TryParse<TaskStatus>(status, true, out var taskStatus))
        {
            var updateDto = new UpdateTaskDto
            {
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                CategoryId = task.CategoryId,
                Priority = task.Priority,
                DueDate = task.DueDate,
                Status = taskStatus
            };

            await _taskService.UpdateTaskAsync(taskId, updateDto);
            TempData["Success"] = "Task status updated!";
        }
        else
        {
            TempData["Error"] = "Invalid status value.";
        }

        return RedirectToAction(nameof(MyTasks));
    }

    [HttpGet]
    public async Task<IActionResult> TaskDetails(int id)
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        // Verify the task belongs to the current user
        if (task.AssignedToUserId != userId.Value && task.CreatedByUserId != userId.Value)
        {
            return Forbid();
        }

        ViewBag.UserName = User.Identity?.Name ?? "User";
        return View(task);
    }

    // GET: /User/Profile
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        await SetSidebarCountsAsync();
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var user = await _userService.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Get user's task stats
        var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);
        var taskList = tasks.ToList();

        ViewBag.TotalTasks = taskList.Count;
        ViewBag.CompletedTasks = taskList.Count(t => t.Status == TaskStatus.Completed);
        ViewBag.InProgressTasks = taskList.Count(t => t.Status == TaskStatus.InProgress);
        ViewBag.OverdueTasks = taskList.Count(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed);

        return View(user);
    }

    // POST: /User/UpdateProfile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var currentUser = await _userService.GetByIdAsync(userId.Value);
        if (currentUser == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Profile));
        }


        var updateDto = new UpdateUserDto
        {
            Id = userId.Value,
            FullName = dto.FullName,
            Email = dto.Email,
            Status = UserStatus.Active  
        };

        await _userService.UpdateAsync(updateDto);

        TempData["Success"] = "Profile updated successfully!";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            TempData["Error"] = "Task not found.";
            return RedirectToAction(nameof(MyTasks));
        }

        if (task.CreatedByUserId != userId.Value && task.AssignedToUserId != userId.Value)
        {
            TempData["Error"] = "You don't have permission to delete this task.";
            return RedirectToAction(nameof(MyTasks));
        }

        var success = await _taskService.DeleteTaskAsync(id);
        TempData[success ? "Success" : "Error"] = success ? "Task deleted successfully." : "Failed to delete task.";

        return RedirectToAction(nameof(MyTasks));
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var (success, error) = await _authService.ChangePasswordAsync(userId.Value, dto);

        if (!success)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Profile));
        }

        TempData["Success"] = "Password changed successfully!";
        return RedirectToAction(nameof(Profile));
    }
}