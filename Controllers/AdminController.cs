using Microsoft.AspNetCore.Mvc;

namespace TaskManagementSystem.Controllers;

public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Users()
    {
        return View();
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
