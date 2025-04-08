using Microsoft.AspNetCore.Mvc;

namespace AttendanceDatabase.Models
{
        public class FlaggedDataViewModel
        {
            public List<Category> Categories { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }

}
