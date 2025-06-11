using System.Windows;
using Microsoft.EntityFrameworkCore;
using ProjectControl.Data;
using ProjectControl.Desktop.ViewModels;
using ProjectControl.Desktop.Views;

namespace ProjectControl.Desktop;

public partial class MainWindow : Window
{
    private readonly ProjectRepository _repo;
    private readonly CustomerRepository _customerRepo;
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        var options = new DbContextOptionsBuilder<ProjectControlContext>()
            .UseSqlite("Data Source=projects.db")
            .Options;
        var context = new ProjectControlContext(options);
        context.Database.EnsureCreated();
        _repo = new ProjectRepository(context);
        _customerRepo = new CustomerRepository(context);
        _vm = new MainViewModel(_repo);
        DataContext = _vm;
        _ = _vm.LoadProjectsAsync();
    }

    private void OnAddProject(object sender, RoutedEventArgs e)
    {
        var editorVm = new ProjectEditorViewModel(_repo);
        var win = new ProjectEditorWindow(editorVm);
        editorVm.Saved += async () =>
        {
            await _vm.LoadProjectsAsync();
            win.Close();
        };
        win.ShowDialog();
    }

    private async void OnCustomers(object sender, RoutedEventArgs e)
    {
        var listVm = new CustomerListViewModel(_customerRepo);
        await listVm.LoadCustomersAsync();
        var win = new CustomerListWindow(listVm);
        listVm.NewCustomer += () =>
        {
            var editorVm = new CustomerEditorViewModel(_customerRepo);
            var editWin = new CustomerEditorWindow(editorVm);
            editorVm.Saved += async () =>
            {
                await listVm.LoadCustomersAsync();
                editWin.Close();
            };
            editWin.ShowDialog();
        };
        win.ShowDialog();
    }
}
