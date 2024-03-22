using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class HomeController : Controller
{ 

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Index()
    {
        return View();
    }
}
