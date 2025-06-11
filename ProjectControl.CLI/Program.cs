using Microsoft.EntityFrameworkCore;
using ProjectControl.Core.Models;
using ProjectControl.Data;

Console.WriteLine("ProjectControl CLI demo");

var options = new DbContextOptionsBuilder<ProjectControlContext>()
    .UseSqlite("Data Source=projects.db")
    .Options;

using var context = new ProjectControlContext(options);
await context.Database.EnsureCreatedAsync();

var repo = new ProjectRepository(context);

var project = new Project { Name = "Demo", CustomerId = 1 };
await repo.AddProjectAsync(project);
Console.WriteLine($"Создан проект с Id {project.Id}");

await repo.StartTimerAsync(project.Id);
Console.WriteLine("Таймер запущен...");
await Task.Delay(1000);
await repo.PauseTimerAsync(project.Id);
Console.WriteLine("Таймер остановлен. Всего секунд: " + project.TotalTimeSpent);

var projects = await repo.GetProjectsAsync();
Console.WriteLine($"В базе проектов: {projects.Count}");

