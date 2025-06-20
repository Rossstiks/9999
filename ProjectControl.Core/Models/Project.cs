using System;
using System.Collections.Generic;

namespace ProjectControl.Core.Models
{
    public class Project
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;
        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;
        public long TotalTimeSpent { get; set; }
        public DateTime? CurrentTimerStartTime { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public double? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentNotes { get; set; }
        public List<string>? Tags { get; set; }
        public double? EstimatedBudget { get; set; }
        public string? ProjectLink { get; set; }
    }
}
