using ProjectControl.Core.Models;
Console.WriteLine("ProjectControl CLI");
// Это простой пример использования моделей
var project = new Project
{
    Id = 1,
    Name = "Пример проекта",
    CustomerId = 1
};
Console.WriteLine($"Создан проект: {project.Name}, статус: {project.Status}");
