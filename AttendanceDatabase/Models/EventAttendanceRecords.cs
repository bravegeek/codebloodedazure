using System;

namespace AttendanceDatabase.Models
{
    public class EventAttendanceRecord
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime Date { get; set; }
        public bool? Event { get; set; }
        public bool? Program { get; set; }
        public bool? Cafe { get; set; }
        public string? Tags { get; set; }
        public int AttendanceCount { get; set; }
        public bool IsFlagged { get; set; }
    }

}
