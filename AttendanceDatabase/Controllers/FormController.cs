using System.Data;
using System.Diagnostics;
using AttendanceDatabase.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using AttendanceDatabase.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceDatabase.Controllers
{
    public class FormController : Controller
    {
        private readonly ILogger<FormController> _logger;
        private readonly AttendanceDbContext _context;


        public FormController(ILogger<FormController> logger, AttendanceDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult RP_Index() { return View(); }


        /*Showing filtered categories based on the given requirements*/
        [HttpPost]
        public IActionResult RP_ViewPastCategories(DateTime? startDate, DateTime? endDate, string? categoryTag, bool? events, bool? programs, bool? cafes)
        {
            var categoryList = _context.EventAttendanceRecords.AsQueryable();
            DateTime today = DateTime.Today;

            if (startDate.HasValue)
            {
                if (endDate.HasValue) { }
                else
                {
                    endDate = today;
                }

                categoryList = categoryList.Where(v => (v.Date.Date >= startDate.Value.Date) & (v.Date.Date <= endDate.Value.Date));
            }
            if (!string.IsNullOrEmpty(categoryTag))
            {
                categoryList = categoryList.Where(v => v.Tags.Contains(categoryTag, StringComparison.OrdinalIgnoreCase));
            }
            if (events == true)
            {
                categoryList = categoryList.Where(v => v.Event == true);
            }
            if (programs == true)
            {
                categoryList = categoryList.Where(v => v.Program == true);
            }
            if (cafes == true)
            {
                categoryList = categoryList.Where(v => v.Cafe == true);
            }

            var filteredCategories = categoryList.ToList();
            return View(filteredCategories);
        }

        /*Gathering selected events to create and display visuals {WIP}*/
        public IActionResult RP_VisualReportPage(string? selectedCategoryIds, List<int>? category)
        {
            List<EventAttendanceRecord> selectedCategories = new List<EventAttendanceRecord>();

            if (!string.IsNullOrEmpty(selectedCategoryIds))
            {
                var selectedIdsArray = selectedCategoryIds?.Split(',');
                List<Int32> catList = new List<Int32>();

                foreach (var item in selectedIdsArray)
                {
                    int x = 0;
                    Int32.TryParse(item, out x);
                    catList.Add(x);
                }

                foreach (var item in catList)
                {
                    selectedCategories.Add(_context.EventAttendanceRecords.SingleOrDefault(c => c.Id == item));
                }
            }
            else if (category != null && category.Any())
            {
                foreach (var id in category)
                {
                    selectedCategories.Add(_context.EventAttendanceRecords.SingleOrDefault(c => c.Id == id));
                }
            }
            return View(selectedCategories);
        }

        /*Gathering selected events to have their raw data downloaded to an Excel file*/
        public async Task<FileResult> RP_DownloadRawData(string? selectedCategoryIds, List<int>? category)
        {

            List<EventAttendanceRecord> selectedCategories = new List<EventAttendanceRecord>();

            if (!string.IsNullOrEmpty(selectedCategoryIds))
            {
                var selectedIdsArray = selectedCategoryIds?.Split(',');
                List<Int32> catList = new List<Int32>();

                foreach (var item in selectedIdsArray)
                {
                    int x = 0;
                    Int32.TryParse(item, out x);
                    catList.Add(x);
                }

                foreach (var item in catList)
                {
                    selectedCategories.Add(_context.EventAttendanceRecords.SingleOrDefault(c => c.Id == item));
                }
            }
            else if (category != null && category.Any())
            {
                foreach (var id in category)
                {
                    selectedCategories.Add(_context.EventAttendanceRecords.SingleOrDefault(c => c.Id == id));
                }
            }

            var fileName = "Category Attendance Data";

            return GenerateExcel(fileName, selectedCategories);
        }

        /*Generating the Excel file to download to computer*/
        private FileResult GenerateExcel(string fileName, IEnumerable<EventAttendanceRecord> selectedCategories)
        {
            DataTable dataTable = new DataTable("Categories");
            dataTable.Columns.AddRange(new DataColumn[]{
            new DataColumn("Name"),
            new DataColumn("Date"),
            new DataColumn("Tags"),
            new DataColumn("Event"),
            new DataColumn("Program"),
            new DataColumn("Cafe"),
            new DataColumn("Attendance Count")
        });

            foreach (var category in selectedCategories)
            {
                dataTable.Rows.Add(category.EventName, category.Date, category.Tags, category.Event, category.Program, category.Cafe, category.AttendanceCount);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    stream.Seek(0, SeekOrigin.Begin);  // Ensure we're at the start of the stream

                    // Convert the MemoryStream to a byte array
                    byte[] fileBytes = stream.ToArray();

                    // Return the file to the client with appropriate headers
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
                }
            }
        }



        /*Event Management TO BE DELETED LATER*/

        public IActionResult RP_Category(int? id)
        {
            if (id != null)
            {//editing -> load an expense by id
                var categoryInDb = _context.EventAttendanceRecords.SingleOrDefault(categories => categories.Id == id);
                return View(categoryInDb);
            }
            return View();
        }

        // Display all products
        public IActionResult RP_ViewCategories()
        {
            var categoryList = _context.EventAttendanceRecords.ToList();
            return View(categoryList);
        }

        //Add or Edit event
        [HttpPost]
        public IActionResult RP_AddEditCategoryForm(EventAttendanceRecord model)
        {
            if (model.Id == 0)
            {
                //Adding
                _context.EventAttendanceRecords.Add(model);
            }
            else
            {
                //Editing
                _context.EventAttendanceRecords.Update(model);
            }

            _context.SaveChanges(); //Must be done or else statement above doesnt work

            return RedirectToAction("RP_ViewCategories");
        }

        public IActionResult RP_DeleteCategory(int id)
        {
            var categoryInDb = _context.EventAttendanceRecords.SingleOrDefault(categories => categories.Id == id); //find where an id in the Db matches the given id
            _context.EventAttendanceRecords.Remove(categoryInDb); //remove that expense from the DB
            _context.SaveChanges();
            return RedirectToAction("RP_ViewCategories");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*Editing event*/
        public IActionResult EditCategory(EventAttendanceRecord model)
        {
            //Editing
            _context.EventAttendanceRecords.Update(model);
            _context.SaveChanges(); //Must be done or else statement above doesnt work

            return RedirectToAction("RP_ViewCategories");
        }

        /*Delete event*/
        public IActionResult DeleteCategory(int id)
        {
            var categoryInDb = _context.EventAttendanceRecords.SingleOrDefault(categories => categories.Id == id); //find where an id in the Db matches the given id
            _context.EventAttendanceRecords.Remove(categoryInDb); //remove that expense from the DB
            _context.SaveChanges();
            return RedirectToAction("RP_ViewCategories");
        }
    }
}
