using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ProjectControl.Core.Models;
using ProjectControl.Data;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

using System.ComponentModel;

public class ProjectEditorViewModel : INotifyPropertyChanged
{
    private readonly ProjectRepository _repo;
    private readonly CustomerRepository _customerRepo;
    public Project Project { get; }

    public DelegateCommand SaveCommand { get; }

    public event Action? Saved;
    public event PropertyChangedEventHandler? PropertyChanged;

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
            OnPropertyChanged(nameof(SelectedCustomer));
        }
    }

    public ProjectEditorViewModel(ProjectRepository repo, CustomerRepository customerRepo, Project? project = null)
    {
        _repo = repo;
        _customerRepo = customerRepo;
        Project = project ?? new Project();
        SaveCommand = new DelegateCommand(async _ => await SaveAsync());
    }

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public async Task LoadCustomersAsync()
    {
        Customers.Clear();
        foreach (var c in await _customerRepo.GetCustomersAsync())
            Customers.Add(c);
        SelectedCustomer = Customers.FirstOrDefault(c => c.Id == Project.CustomerId);
    }

    private async Task SaveAsync()
    {
        if (SelectedCustomer != null)
        {
            Project.CustomerId = SelectedCustomer.Id;
            Project.Customer = SelectedCustomer;
        }
        if (Project.Id == 0)
            await _repo.AddProjectAsync(Project);
        else
            await _repo.UpdateProjectAsync(Project);
        Saved?.Invoke();
    }
}
