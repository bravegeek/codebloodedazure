using System;
namespace AttendanceDatabase.Models
{
	public class CategoryInfo
	{
		public Category Category { get; set; }
		public List<Tags> Tags { get; set; }
		public CategoryTags CategoryTags { get; set; }
	}
}

