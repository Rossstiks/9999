using System;

namespace ProjectControl.Core.Models
{
    public class TimeEntry
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long Duration { get; set; }
        public string? Notes { get; set; }
    }
}
