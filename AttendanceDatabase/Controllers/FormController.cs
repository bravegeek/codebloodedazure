//REPORT GENERATION

/*Showing filtered categories based on the given requirements*/

using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;
using System.Data;
using System.Diagnostics;

namespace AttendanceDatabase.Controllers;

public class FormController : Controller
{
    private readonly ILogger<FormController> _logger;
    private readonly AppDbContext _context;


    public FormController(ILogger<FormController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult RP_Index()
    {
        if(HttpContext.Session.GetString("_Role") == "Admin")
        {
            return RedirectToAction("Admin_Index", "Home");
        }
        else
        {
            return RedirectToAction("Staff_Index", "Home");
        }
    }

    /*Showing filtered categories based on the given requirements*/

    public IActionResult ViewPastCategories(List<String> selectedTags, string? selectedName, DateTime? startDate, DateTime? endDate, bool? events, bool? programs, bool? cafes, int page = 1)
    {
        const int pageSize = 25;

        var filteredCategories = _context.Categories.AsQueryable();

        // Apply filters for Category Types
        if (events.HasValue)
            filteredCategories = filteredCategories.Where(c => c.Event == events.Value);

        if (programs.HasValue)
            filteredCategories = filteredCategories.Where(c => c.Program == programs.Value);

        if (cafes.HasValue)
            filteredCategories = filteredCategories.Where(c => c.Cafe == cafes.Value);

        // Apply filters for Date Range
        if (startDate.HasValue && endDate.HasValue)
            filteredCategories = filteredCategories.Where(c => c.Date >= startDate.Value && c.Date <= endDate.Value);

        // Apply filter for selected name
        if (!string.IsNullOrEmpty(selectedName))
            filteredCategories = filteredCategories.Where(c => c.Name.Contains(selectedName));

        // Apply filter for selected tags
        if (selectedTags != null && selectedTags.Any())
        {
            filteredCategories = filteredCategories.Where(c => c.Tags.Any(t => selectedTags.Contains(t.TagName)));
        }

        // Sort by Date in descending order
        filteredCategories = filteredCategories.OrderByDescending(c => c.Date);

        // Include related Tags while filtering
        var categoryInfoList = filteredCategories.Include(c => c.Tags).ToList()
            .Select(c => new CategoryInfo
            {
                Category = c,
                Tags = c.Tags.Select(ct => new Tags { Name = ct.TagName }).ToList()
            }).ToList();

        // Pagination logic
        var totalItems = categoryInfoList.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var categoriesForCurrentPage = categoryInfoList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Pass pagination data along with category data to the view
        var model = new CategoryPaginationViewModel
        {
            Categories = categoriesForCurrentPage,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(model);
    }

    /*Displays a bar graph and a Pie chart based on the data provided from selected Category Events*/
    public async Task<IActionResult> VisualReportPage(string? selectedCategoryIds, List<int>? category)
    {
        List<Category> selectedCategories = new List<Category>();
        int totalAttn = 0;

        if (!string.IsNullOrEmpty(selectedCategoryIds))
        {
            var selectedIdsArray = selectedCategoryIds.Split(',');
            List<int> catList = selectedIdsArray.Select(id => int.TryParse(id, out var result) ? result : 0).ToList();
            selectedCategories = _context.Categories.Include(c => c.Tags).Where(c => catList.Contains(c.Id)).ToList();
        }
        else if (category != null && category.Any())
        {
            selectedCategories = _context.Categories.Include(c => c.Tags).Where(c => category.Contains(c.Id)).ToList();
        }

        // Check if selectedCategories contains any data
        if (selectedCategories.Count == 0)
        {
            // Handle empty categories case
            return View("Error", new { message = "No categories selected." });
        }

        // Total Attendance Calculation
        totalAttn = selectedCategories.Sum(c => c.TotAttn);

        List<CategoryTags> pieData = new List<CategoryTags>();

        foreach (var cat in selectedCategories)
        {

            foreach (var categoryTag in cat.Tags)
            {
                var existingTag = pieData.FirstOrDefault(pt => pt.TagName == categoryTag.TagName);

                if (existingTag != null)
                {
                    existingTag.AttnCnt += categoryTag.AttnCnt;
                }
                else
                {
                    pieData.Add(new CategoryTags
                    {
                        TagName = categoryTag.TagName,
                        AttnCnt = categoryTag.AttnCnt
                    });
                }
            }
        }

        // Pie Chart Data
        var pieChartData = pieData.Select(tag => new ChartDataViewModel
        {
            Tag = tag.TagName,
            Count = tag.AttnCnt,
            Percentage = (double)tag.AttnCnt / totalAttn * 100
        }).ToList();


        // Bar Chart Data ordered chronologically
        var barChartData = selectedCategories.OrderBy(c => c.Date).ToList();

        // ViewBag Items for chart data
        ViewBag.TagCnt = _context.Tags.ToList().Count();
        ViewBag.CategoryCnt = selectedCategories.Count();
        ViewBag.TotCount = totalAttn;
        // Set ViewBag for pie chart
        ViewBag.PieChartTags = pieChartData.Select(c => c.Tag).ToArray();
        ViewBag.PieChartAttnCnts = pieChartData.Select(c => c.Count).ToArray();

        // Set ViewBag for bar chart
        ViewBag.BarChartNames = barChartData.Select(c => c.Name).ToArray();
        ViewBag.BarChartAttnCnts = barChartData.Select(c => c.TotAttn).ToArray();

        return View(pieChartData);
    }

    /*Gathering selected events to have their raw data downloaded to an Excel file*/
    public async Task<FileResult> DownloadRawData(string? selectedCategoryIds, List<int>? category)
    {
        List<Category> selectedCategories = new List<Category>();
        int totalAttn = 0;

        if (!string.IsNullOrEmpty(selectedCategoryIds))
        {
            var selectedIdsArray = selectedCategoryIds.Split(',');
            List<int> catList = new List<int>();

            foreach (var item in selectedIdsArray)
            {
                int x = 0;
                Int32.TryParse(item, out x);
                catList.Add(x);
            }

            selectedCategories = _context.Categories.Include(c => c.Tags).Where(c => catList.Contains(c.Id)).ToList();
        }
        else if (category != null && category.Any())
        {
            selectedCategories = _context.Categories.Include(c => c.Tags).Where(c => category.Contains(c.Id)).ToList();
        }

        if (!selectedCategories.Any())
        {
            // Return an error file if no categories are selected.
            return File(new byte[0], "application/octet-stream", "NoCategoriesSelected.xlsx");
        }


        //Organize Tag Attendance Data

        // Total Attendance Calculation
        totalAttn = selectedCategories.Sum(c => c.TotAttn);

        List<CategoryTags> pieData = new List<CategoryTags>();

        foreach (var cat in selectedCategories)
        {
            foreach (var categoryTag in cat.Tags)
            {
                var existingTag = pieData.FirstOrDefault(pt => pt.TagName == categoryTag.TagName);

                if (existingTag != null)
                {
                    existingTag.AttnCnt += categoryTag.AttnCnt;
                }
                else
                {
                    pieData.Add(new CategoryTags
                    {
                        TagName = categoryTag.TagName,
                        AttnCnt = categoryTag.AttnCnt
                    });
                }
            }
        }




        //Organize Event Attendance Data

        string date = DateTime.Now.ToString("MM.dd.yyyy hh:mm");

        var fileName = date + "_Attendance_Data.xlsx";

        return GenerateExcel(fileName, selectedCategories, pieData);
    }

    /*Generating the Excel file to download to computer*/
    private FileResult GenerateExcel(string fileName, IEnumerable<Category> selectedCategories, List<CategoryTags> pieData)
    {
        DataTable dataTable = new DataTable("Category Attendance Data");
        dataTable.Columns.AddRange(new DataColumn[]{
        new DataColumn("Name"),
        new DataColumn("Date"),
        new DataColumn("Tags"),
        new DataColumn("Event"),
        new DataColumn("Program"),
        new DataColumn("Cafe"),
        new DataColumn("Attendance Count")
        });

        // Pie Chart Data
        DataTable pieChartDataTable = new DataTable("Tag Attendance Data");
        pieChartDataTable.Columns.AddRange(new DataColumn[] {
        new DataColumn("Tags"),
        new DataColumn("Attendance Count")
        });


        foreach (var pieTag in pieData)
        {
            pieChartDataTable.Rows.Add(pieTag.TagName, pieTag.AttnCnt);
        }

        foreach (var category in selectedCategories)
        {
            // Convert the list of tags to a comma-separated string
            string tags = category.Tags != null && category.Tags.Any() ? string.Join(", ", category.Tags.Select(t => t.TagName)) : "No Tags";

            dataTable.Rows.Add(
                category.Name,
                category.Date.ToString("MM/dd/yyyy"),
                tags,
                category.Event ? "Yes" : "No",
                category.Program ? "Yes" : "No",
                category.Cafe ? "Yes" : "No",
                category.TotAttn
            );
        }

        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.Worksheets.Add(dataTable);
            wb.Worksheets.Add(pieChartDataTable);
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public IActionResult Error()
{
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}


}