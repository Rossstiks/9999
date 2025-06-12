using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using ProjectControl.Core.Models;
using ProjectControl.Data;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public class CustomerListViewModel : INotifyPropertyChanged
{
    private readonly CustomerRepository _repo;

    public ObservableCollection<Customer> Customers { get; } = new();
    public DelegateCommand NewCustomerCommand { get; }
    public DelegateCommand DeleteCustomerCommand { get; }

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            DeleteCustomerCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(SelectedCustomer));
        }
    }

    public CustomerListViewModel(CustomerRepository repo)
    {
        _repo = repo;
        NewCustomerCommand = new DelegateCommand(_ => NewCustomer?.Invoke());
        DeleteCustomerCommand = new DelegateCommand(async _ => await DeleteAsync(), _ => SelectedCustomer != null);
    }

    private async Task DeleteAsync()
    {
        if (SelectedCustomer == null) return;
        await _repo.DeleteCustomerAsync(SelectedCustomer.Id);
        await LoadCustomersAsync();
    }

    public event System.Action? NewCustomer;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public async Task LoadCustomersAsync()
    {
        Customers.Clear();
        foreach (var c in await _repo.GetCustomersAsync())
            Customers.Add(c);
    }
}
