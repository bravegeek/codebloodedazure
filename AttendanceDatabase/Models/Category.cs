using System;
using System.ComponentModel.DataAnnotations;
namespace AttendanceDatabase.Models
{
	public class Category
	{
		public int Id { get; set; }
        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public bool Event { get; set; }
        public bool Program { get; set; }
        public bool Cafe { get; set; }
        public List<CategoryTags> Tags { get; set; } = new List<CategoryTags>();
        public int TotAttn { get; set; }
		public bool Flagged { get; set; }
	}
}

