using System;
using System.ComponentModel.DataAnnotations;
namespace AttendanceDatabase.Models
{
    public class Tags
    {
        public int Id { get; set; }

        [Required]
        public String Name { get; set; }
    }
}

