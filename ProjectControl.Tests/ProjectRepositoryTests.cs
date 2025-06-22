using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectControl.Core.Models;
using ProjectControl.Data;
using Xunit;

public class ProjectRepositoryTests
{
    private static ProjectControlContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProjectControlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProjectControlContext(options);
    }

    [Fact]
    public async Task StartTimerCreatesTimeEntry()
    {
        using var context = CreateContext();
        var repo = new ProjectRepository(context);
        var project = new Project { Name = "Test" };
        await repo.AddProjectAsync(project);

        await repo.StartTimerAsync(project.Id);
        await Task.Delay(10);
        await repo.PauseTimerAsync(project.Id);

        Assert.Single(await context.TimeEntries.ToListAsync());
    }

    [Fact]
    public async Task CompleteProjectSetsStatusAndPayment()
    {
        using var context = CreateContext();
        var repo = new ProjectRepository(context);
        var project = new Project { Name = "Test" };
        await repo.AddProjectAsync(project);

        await repo.StartTimerAsync(project.Id);
        await Task.Delay(10);
        await repo.CompleteProjectAsync(project.Id, PaymentStatus.Paid, 50, System.DateTime.Today, "done");

        var updated = await context.Projects.FindAsync(project.Id);
        Assert.NotNull(updated);
        Assert.Equal(ProjectStatus.Completed, updated!.Status);
        Assert.Equal(PaymentStatus.Paid, updated.PaymentStatus);
        Assert.Equal(50, updated.PaymentAmount);
        Assert.NotNull(updated.ActualCompletionDate);
    }

    [Fact]
    public async Task CompleteProjectRequiresAmount()
    {
        using var context = CreateContext();
        var repo = new ProjectRepository(context);
        var project = new Project { Name = "Test" };
        await repo.AddProjectAsync(project);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await repo.CompleteProjectAsync(project.Id, PaymentStatus.Paid, 0, null, null));
    }

    [Fact]
    public async Task AddProjectWithCustomerPersistsCustomerId()
    {
        using var context = CreateContext();
        var repo = new ProjectRepository(context);
        var customer = new Customer { Name = "Cust" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var project = new Project { Name = "Test", CustomerId = customer.Id };
        await repo.AddProjectAsync(project);

        var projects = await repo.GetProjectsWithCustomerAsync();
        Assert.Single(projects);
        Assert.Equal(customer.Id, projects[0].CustomerId);
        Assert.Equal("Cust", projects[0].Customer?.Name);
    }

    [Fact]
    public async Task FilterProjectsByPeriodCustomerAndAmount()
    {
        using var context = CreateContext();
        var repo = new ProjectRepository(context);
        var customer = new Customer { Name = "Cust" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var p1 = new Project
        {
            Name = "P1",
            CustomerId = customer.Id,
            Status = ProjectStatus.Completed,
            ActualCompletionDate = new DateTime(2024, 1, 1),
            PaymentAmount = 100
        };
        var p2 = new Project
        {
            Name = "P2",
            CustomerId = customer.Id,
            Status = ProjectStatus.Completed,
            ActualCompletionDate = new DateTime(2024, 2, 1),
            PaymentAmount = 200
        };
        await repo.AddProjectAsync(p1);
        await repo.AddProjectAsync(p2);

        var result = await repo.GetProjectsWithCustomerAsync(
            ProjectStatus.Completed,
            new DateTime(2024, 1, 15),
            new DateTime(2024, 12, 31),
            customer.Id,
            150,
            250);

        Assert.Single(result);
        Assert.Equal("P2", result[0].Name);
    }
}
