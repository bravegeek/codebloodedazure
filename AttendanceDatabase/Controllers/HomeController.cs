using System.Data;
using System.Diagnostics;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;

namespace AttendanceDatabase.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    // Inject AppDbContext through constructor
    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Admin_Index()
    {
        if (HttpContext.Session.GetString("_Role") == "Admin")
        {
            var categoryTagCount = _context.Tags.Count();
            var categoryEventCount = _context.Categories.Count();
            var accountCount = _context.Accounts.Count();

            ViewBag.username = HttpContext.Session.GetString("_Name");



            ViewBag.CategoryTagCount = categoryTagCount;
            ViewBag.CategoryEventCount = categoryEventCount;
            ViewBag.AccountCount = accountCount;

            ViewData["HideNavbar"] = true;

            return View();
        }
        else
        {
            return NotFound();
        }

            
    }

    public IActionResult Staff_Index()
    {
        ViewBag.username = HttpContext.Session.GetString("_Name");
        var categoryEventCount = _context.Categories.Count();
        ViewBag.CategoryEventCount = categoryEventCount;
        ViewData["HideNavbar"] = true;
        return View();
    }

}