namespace ProjectControl.Core.Models
{
    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
    }
}
