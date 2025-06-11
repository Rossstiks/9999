using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ProjectControl.Core.Models;
using ProjectControl.Data;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public class ProjectEditorViewModel
{
    private readonly ProjectRepository _repo;
    private readonly CustomerRepository _customerRepo;
    public Project Project { get; }

    public DelegateCommand SaveCommand { get; }

    public event Action? Saved;

    public ObservableCollection<Customer> Customers { get; } = new();
    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            if (value != null)
                Project.CustomerId = value.Id;
        }
    }

    public ProjectEditorViewModel(ProjectRepository repo, CustomerRepository customerRepo, Project? project = null)
    {
        _repo = repo;
        _customerRepo = customerRepo;
        Project = project ?? new Project();
        SaveCommand = new DelegateCommand(async _ => await SaveAsync());
    }

    public async Task LoadCustomersAsync()
    {
        Customers.Clear();
        foreach (var c in await _customerRepo.GetCustomersAsync())
            Customers.Add(c);
        SelectedCustomer = Customers.FirstOrDefault(c => c.Id == Project.CustomerId);
    }

    private async Task SaveAsync()
    {
        if (Project.Id == 0)
            await _repo.AddProjectAsync(Project);
        else
            await _repo.UpdateProjectAsync(Project);
        Saved?.Invoke();
    }
}
