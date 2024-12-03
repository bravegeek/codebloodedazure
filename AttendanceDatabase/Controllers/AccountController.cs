using Microsoft.AspNetCore.Mvc;
using AttendanceDatabase.Data;
using AttendanceDatabase.Models;
using System.Linq;

namespace AttendanceDatabase.Controllers
{
    public class AccountController : Controller
    {
        private readonly AttendanceDbContext _context;

        public AccountController(AttendanceDbContext context)
        {
            _context = context;
        }

        public IActionResult ManageAccounts()
        {
            var accounts = _context.Accounts.ToList();
            return View("~/Views/Accounts/ManageAccounts.cshtml", accounts);
        }

        public IActionResult Create()
        {
            return View("~/Views/Accounts/Create.cshtml");
        }

        [HttpPost]
        public IActionResult Create(Account account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Accounts.Add(account);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Account created successfully.";
                    return RedirectToAction("ManageAccounts");
                }
                else
                {
                    TempData["ErrorMessage"] = "Please correct the errors and try again.";
                    return View("~/Views/Accounts/Create.cshtml", account);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View("~/Views/Accounts/Create.cshtml", account);
            }
        }
    }
}
