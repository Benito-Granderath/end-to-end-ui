using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class HomeController : Controller
{ 

    public HomeController()
    {
        }

    public IActionResult Index()
    {
        return View();
    }
}
