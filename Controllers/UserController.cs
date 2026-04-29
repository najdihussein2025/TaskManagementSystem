using Microsoft.AspNetCore.Mvc;

namespace TaskManagementSystem.Controllers;

public class UserController : Controller
{
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
    public IActionResult MyTasks()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CreateTask()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UpdateTaskStatus(int taskId, string status)
    {
        return RedirectToAction(nameof(MyTasks));
    }
}
