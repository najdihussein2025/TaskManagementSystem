using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagementSystem.Controllers;

[Authorize(AuthenticationSchemes = "JwtBearer", Roles = "User")]
public class UserController : Controller
{
    public UserController() { }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }

    [HttpGet]
    public IActionResult MyTasks()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CreateTask()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UpdateTaskStatus()
    {
        return RedirectToAction(nameof(MyTasks));
    }
}
