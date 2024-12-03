using AttendanceDatabase.Data;
using AttendanceDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AttendanceDatabase.Controllers
{
    public class EditCalendarController : Controller
    {
        private readonly AttendanceDbContext _context;

        public EditCalendarController(AttendanceDbContext context)
        {
            _context = context;
        }

        // GET: EditCalendar
        public IActionResult EditCalendar()
        {
            return View();
        }

        // POST: EditCalendar - submits the selected date
        [HttpPost]
        public IActionResult SubmitDate(DateTime selectedDate)
        {
            // Format the date to "yyyy-MM-dd" for clean routing
            string formattedDate = selectedDate.ToString("yyyy-MM-dd");

            // Redirect to EditData with the selected date as a route parameter
            return RedirectToAction("EditData", "DataEntry", new { entryDate = formattedDate });
        }
    }
}
