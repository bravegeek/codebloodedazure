using AttendanceDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceDatabase.Controllers
{
    public class MenusController : Controller
    {
        public IActionResult MainMenu() { return View(); }
        [HttpPost]
        public IActionResult MainMenu(Account account)
        {
            if (account.Username == "Admin" && account.Password == "password") {
                return View();
            }
            else
            {
                ViewBag.message = "Incorect username and password";
                return View("~/Views/Home/Index.cshtml");
            }
           
        }

        public IActionResult DataEntryMenu()
        {
            return View();
        }

        public IActionResult DailyEntry()
        {
            return View("~/Views/DataEntry/DailyEntry.cshtml");
        }

        public IActionResult EditCalendar()
        {
            return View("~/Views/EditCalendar/EditCalendar.cshtml");
        }

        public IActionResult RP_ViewCategories()
        {
            return View();
        }
        public IActionResult ManageAccounts()
        {
            return View("~/Views/Accounts/ManageAccounts.cshtml");
        }
    }
}
