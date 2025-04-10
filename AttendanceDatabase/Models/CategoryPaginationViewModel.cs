using AttendanceDatabase.Models;
using System;
namespace AttendanceDatabase.Models
{
    public class CategoryPaginationViewModel
    {
        public List<CategoryInfo> Categories { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
