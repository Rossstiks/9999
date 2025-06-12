using Microsoft.EntityFrameworkCore;
using ProjectControl.Core.Models;

namespace ProjectControl.Data;

public class ProjectControlContext : DbContext
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    public ProjectControlContext(DbContextOptions<ProjectControlContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Customer)
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimeEntry>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
