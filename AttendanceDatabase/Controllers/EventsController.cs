using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AttendanceDatabase.Models;
using AttendanceDatabase.Data;

namespace AttendanceDatabase.Controllers
{
    public class EventsController : Controller
    {
        private readonly AttendanceDbContext _context;

        public EventsController(AttendanceDbContext context)
        {
            _context = context;
        }

        // GET: Display the event creator form
        public IActionResult EventCreator()
        {
            var events = _context.EventAttendanceRecords.ToList();
            return View(events); // Pass the list of events to the view
        }

        // POST: Create a new event
        [HttpPost]
        public IActionResult Create(string eventName, bool? eventType, bool? program, bool? cafe, DateTime date, string? tags, int numberOfWeeks)
        {
            if (ModelState.IsValid) // Ensure the form input is valid
            {
                try
                {
                    for (int i = 0; i < numberOfWeeks; i++)
                    {
                        var newEvent = new EventAttendanceRecord
                        {
                            EventName = eventName,
                            Date = date.AddDays(i * 7),
                            Tags = tags,
                            AttendanceCount = 0,
                            Event = eventType,
                            Program = program,
                            Cafe = cafe
                        };

                        _context.EventAttendanceRecords.Add(newEvent); // Add each event instance to the database
                    }

                    _context.SaveChanges(); // Commit all changes to the database
                    TempData["SuccessMessage"] = "Events created successfully!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }

                return RedirectToAction("EventCreator");
            }

            TempData["ErrorMessage"] = "Invalid input. Please try again.";
            var events = _context.EventAttendanceRecords.ToList();
            return View("EventCreator", events); // Reload the form with existing events
        }
    }
}
