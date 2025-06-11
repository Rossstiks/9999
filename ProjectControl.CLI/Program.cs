using Microsoft.EntityFrameworkCore;
using ProjectControl.Core.Models;
using ProjectControl.Data;

Console.WriteLine("ProjectControl CLI demo");

var options = new DbContextOptionsBuilder<ProjectControlContext>()
    .UseSqlite("Data Source=projects.db")
    .Options;

using var context = new ProjectControlContext(options);
await context.Database.EnsureCreatedAsync();

var projectRepo = new ProjectRepository(context);
var customerRepo = new CustomerRepository(context);

// Добавим заказчика при первом запуске
var customer = new Customer { Name = "Demo customer" };
await customerRepo.AddCustomerAsync(customer);
Console.WriteLine($"Создан заказчик с Id {customer.Id}");

var project = new Project { Name = "Demo", CustomerId = customer.Id };
await projectRepo.AddProjectAsync(project);
Console.WriteLine($"Создан проект с Id {project.Id}");

await projectRepo.StartTimerAsync(project.Id);
Console.WriteLine("Таймер запущен...");
await Task.Delay(1000);
await projectRepo.PauseTimerAsync(project.Id);
Console.WriteLine("Таймер остановлен. Всего секунд: " + project.TotalTimeSpent);

var projects = await projectRepo.GetProjectsAsync();
Console.WriteLine($"В базе проектов: {projects.Count}");

