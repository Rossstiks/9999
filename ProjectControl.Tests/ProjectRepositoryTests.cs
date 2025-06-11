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
            .UseInMemoryDatabase("testdb")
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
}
