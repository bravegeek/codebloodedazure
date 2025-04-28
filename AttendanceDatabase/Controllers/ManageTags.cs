using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;

namespace AttendanceDatabase.Controllers;

public class ManageTagsController : Controller
{
    private readonly AppDbContext _context;

    // Inject AppDbContext through constructor
    public ManageTagsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult CreateTags()
    {
        if (HttpContext.Session.GetString("_Role") == "Admin")
        {
            return View(new Tags());
        }
        else
        {
            return NotFound();
        } 
    }


    [HttpPost]
    public IActionResult AddTags(Tags model)
    {
        if (HttpContext.Session.GetString("_Role") != "Admin")
        {
            return NotFound();
        }

        if (model == null || string.IsNullOrWhiteSpace(model.Name))
        {
            return View("CreateTags", model);
        }

        if (model.Id == 0)
        {
            // New tag
            _context.Tags.Add(model);
        }
        else
        {
            // Editing existing tag
            var tagInDb = _context.Tags.SingleOrDefault(t => t.Id == model.Id);
            if (tagInDb == null)
            {
                return NotFound();
            }
            tagInDb.Name = model.Name;
            _context.Tags.Update(tagInDb);
        }

        _context.SaveChanges();
        return RedirectToAction("ViewTags");
    }



    public IActionResult ViewTags()
    {
        if (HttpContext.Session.GetString("_Role") == "Admin")
        {
            var tagList = _context.Tags.ToList();

            var tagUsage = new Dictionary<int, bool>();

            foreach (var tag in tagList)
            {
                bool isUsed = _context.Categories.Any(c => c.Tags.Any(t => t.TagName == tag.Name));
                tagUsage[tag.Id] = isUsed;
            }

            ViewBag.TagUsage = tagUsage;
            return View(tagList);
        }
        else
        {
            return NotFound();
        }
            
    }

    public IActionResult DeleteTags(int id)
    {
        if (HttpContext.Session.GetString("_Role") == "Admin")
        {
            var tagInDb = _context.Tags.SingleOrDefault(tags => tags.Id == id);
            _context.Tags.Remove(tagInDb);
            _context.SaveChanges();
            return RedirectToAction("ViewTags");
        }
        else
        {
            return NotFound();
        }
            
    }

    public IActionResult EditTags(int id)
    {
        if (HttpContext.Session.GetString("_Role") == "Admin")
        {
            var tagInDb = _context.Tags.SingleOrDefault(t => t.Id == id);

            if (tagInDb == null)
            {
                return NotFound();
            }

            return View("CreateTags", tagInDb);
        }
        else
        {
            return NotFound();
        }
            
    }

    public IActionResult Staff_ViewTags()
    {
        var tagList = _context.Tags.ToList();
        return View(tagList);
    }



}