using System.Collections.ObjectModel;
using System.Windows;
using ProjectControl.Core.Models;

namespace ProjectControl.Desktop;

public partial class MainWindow : Window
{
    public ObservableCollection<Project> Projects { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Пример начальных данных
        Projects.Add(new Project { Id = 1, Name = "Первый проект", CustomerId = 1 });
        Projects.Add(new Project { Id = 2, Name = "Второй проект", CustomerId = 1 });
    }
}
