using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;

namespace AttendanceDatabase.Controllers;

public class ManageCategoriesController : Controller
{
    private readonly AppDbContext _context;

    // Inject AppDbContext through constructor
    public ManageCategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult CreateCategory(int? id)
    {
        CategoryInfo viewModel;

        if (id != null)
        {
            // Editing mode
            var categoryInDb = _context.Categories
                .Include(c => c.Tags)  // Include related tags
                .SingleOrDefault(categories => categories.Id == id);

            if (categoryInDb == null)
            {
                return NotFound();
            }

            var availableTags = _context.Tags.ToList(); // Get all available tags

            viewModel = new CategoryInfo
            {
                Category = categoryInDb,
                Tags = availableTags  // Set the list of tags
            };
        }
        else
        {
            // Creating a new category with a default date of today and time now
            var newCategory = new Category
            {
                Date = DateTime.Today,  // Default new category date to today
                Time = DateTime.Today.AddHours(12),
                Tags = new List<CategoryTags>()  // Initialize Tags
            };

            var availableTags = _context.Tags.ToList();

            viewModel = new CategoryInfo
            {
                Category = newCategory,
                Tags = availableTags  // Set the list of available tags
            };
        }

        return View(viewModel);
    }

    /*Showing all categories in database*/
    public IActionResult ViewCategories()
    {
        var categoryList = _context.Categories.Include(c => c.Tags).ToList();

        // Create a list to hold all tag names
        List<string> tagNames = new List<string>();

        // Add each tag's name to the list
        foreach (var category in categoryList)
        {
            foreach (var tag in category.Tags)
            {
                // Only add the tag if it's not already in the list
                if (!tagNames.Contains(tag.TagName))
                {
                    tagNames.Add(tag.TagName);
                }
            }
        }

        // Pass the list of tags to the view
        ViewBag.TagNames = tagNames;

        return View(categoryList);
    }

    [HttpPost]
    public IActionResult AddEditCategoryForm(CategoryInfo model, string[] tags)
    {
        var existingCategory = _context.Categories
            .Include(c => c.Tags)
            .SingleOrDefault(c => c.Id == model.Category.Id);

        if (existingCategory == null)
        {
            // Creating a new category
            model.Category.Tags = tags.Select(tagName => new CategoryTags
            {
                TagName = tagName,
                AttnCnt = 0 // Default attendance count
            }).ToList();

            _context.Categories.Add(model.Category);
        }
        else
        {
            // **Preserve & Update Category Type values**
            existingCategory.Name = model.Category.Name;
            existingCategory.Date = model.Category.Date;
            existingCategory.Time = model.Category.Time; // Make sure to update the time
            existingCategory.Event = model.Category.Event;
            existingCategory.Program = model.Category.Program;
            existingCategory.Cafe = model.Category.Cafe;

            _context.Entry(existingCategory).Property(c => c.Event).IsModified = true;
            _context.Entry(existingCategory).Property(c => c.Program).IsModified = true;
            _context.Entry(existingCategory).Property(c => c.Cafe).IsModified = true;
            _context.Entry(existingCategory).Property(c => c.Time).IsModified = true; // Ensure time is updated

            var existingTags = existingCategory.Tags.ToList();

            // **1. Identify and remove tags no longer selected**
            foreach (var existingTag in existingTags)
            {
                if (!tags.Contains(existingTag.TagName))
                {
                    existingCategory.TotAttn -= existingTag.AttnCnt; // Reduce total attendance
                    _context.CategoryTags.Remove(existingTag); // Explicitly remove from DB
                }
            }

            // **2. Add new tags that don’t already exist**
            foreach (var tagName in tags)
            {
                if (!existingCategory.Tags.Any(t => t.TagName == tagName))
                {
                    var newTag = new CategoryTags
                    {
                        TagName = tagName,
                        AttnCnt = 0 // Start fresh for new tags
                    };

                    existingCategory.Tags.Add(newTag);
                }
            }

            _context.Categories.Update(existingCategory);
        }

        _context.SaveChanges();
        return RedirectToAction("ViewCategories");
    }

    public IActionResult EditCategory(int id)
    {
        var categoryInDb = _context.Categories
            .Include(c => c.Tags) // Load associated tags
            .SingleOrDefault(c => c.Id == id);

        if (categoryInDb == null)
        {
            return NotFound();
        }

        var availableTags = _context.Tags.ToList(); // Get all available tags

        var viewModel = new CategoryInfo
        {
            Category = new Category
            {
                Id = categoryInDb.Id,
                Name = categoryInDb.Name,
                Date = categoryInDb.Date,
                Event = categoryInDb.Event,      // ✅ Preserve Event state
                Program = categoryInDb.Program,  // ✅ Preserve Program state
                Cafe = categoryInDb.Cafe,        // ✅ Preserve Cafe state
                Tags = categoryInDb.Tags.ToList()
            },
            Tags = availableTags
        };

        return View("CreateCategory", viewModel); // Reuse the same view as CreateCategory
    }

    public IActionResult DeleteCategory(int id)
    {
        var categoryInDb = _context.Categories.Include(c => c.Tags)
                                              .SingleOrDefault(c => c.Id == id);

        if (categoryInDb == null)
        {
            return NotFound();
        }

        // Remove all associated tags and then delete the category
        _context.Categories.Remove(categoryInDb);
        _context.SaveChanges();

        // Redirect to the category list view
        return RedirectToAction("ViewCategories");
    }
}