using System.Data;
using System.Diagnostics;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;

namespace AttendanceDatabase.Controllers;

public class DataEntryController : Controller
{
    private readonly AppDbContext _context;

    // Inject AppDbContext through constructor
    public DataEntryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult EnterDate()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DataEntry(DateTime date)
    {
        var categories = _context.Categories.Include(c => c.Tags).Where(c => c.Date.Date == date.Date).ToList();

        ViewBag.SelectedDate = date.ToString("MMMM dd, yyyy"); // Format date as "Month DD, YYYY"

        return View(categories);
    }

    /*Saving Attendance Data*/
    [HttpPost]
    public IActionResult SaveAttendance(List<Category> categories)
    {
        if (categories.Count() > 0)
        {
            foreach (var category in categories)
            {
                // Retrieve the existing category from the database, including its tags
                var existingCategory = _context.Categories.Include(c => c.Tags).FirstOrDefault(c => c.Id == category.Id);

                if (existingCategory != null)
                {
                    // Update the TotAttn value (sum of attendance count)
                    existingCategory.TotAttn = category.Tags.Sum(tag => tag.AttnCnt);

                    // Update the AttnCnt for each tag in the category
                    foreach (var tag in category.Tags)
                    {
                        var existingTag = existingCategory.Tags.FirstOrDefault(t => t.Id == tag.Id);
                        if (existingTag != null)
                        {
                            existingTag.AttnCnt = tag.AttnCnt;
                            _context.Entry(existingTag).State = EntityState.Modified;
                        }
                    }

                    // Update the Flagged status from the form submission
                    existingCategory.Flagged = category.Flagged;

                    _context.Categories.Update(existingCategory);
                }
            }

            // Save changes to the database
            _context.SaveChanges();
        }

        return RedirectToAction("EnterDate");
    }

    public IActionResult FlaggedData(int page = 1)
    {
        int pageSize = 10;

        // Retrieve the flagged categories and order them by date
        var flaggedCategories = _context.Categories
            .Include(c => c.Tags)
            .Where(c => c.Flagged) // Only flagged categories
            .OrderBy(c => c.Date) // Order by date
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Get the total count of flagged categories
        int totalItems = _context.Categories.Count(c => c.Flagged);
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        // Create a model to pass data to the view
        var model = new FlaggedDataViewModel
        {
            Categories = flaggedCategories,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(model);
    }

}
