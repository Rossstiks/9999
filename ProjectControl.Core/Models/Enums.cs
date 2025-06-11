namespace ProjectControl.Core.Models
{
    public enum ProjectStatus
    {
        Pending,
        InProgress,
        Paused,
        Completed,
        Cancelled
    }

    public enum ProjectPriority
    {
        Low,
        Medium,
        High
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid,
        PartiallyPaid
    }
}
