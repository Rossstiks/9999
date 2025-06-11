using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProjectControl.Core.Models;
using ProjectControl.Data;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public class CustomerListViewModel
{
    private readonly CustomerRepository _repo;

    public ObservableCollection<Customer> Customers { get; } = new();
    public DelegateCommand NewCustomerCommand { get; }
    public DelegateCommand EditCustomerCommand { get; }
    public DelegateCommand DeleteCustomerCommand { get; }

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            EditCustomerCommand.RaiseCanExecuteChanged();
            DeleteCustomerCommand.RaiseCanExecuteChanged();
        }
    }

    public CustomerListViewModel(CustomerRepository repo)
    {
        _repo = repo;
        NewCustomerCommand = new DelegateCommand(_ => NewCustomer?.Invoke());
        EditCustomerCommand = new DelegateCommand(_ =>
        {
            if (SelectedCustomer != null)
                EditCustomer?.Invoke(SelectedCustomer);
        }, _ => SelectedCustomer != null);
        DeleteCustomerCommand = new DelegateCommand(async _ => await DeleteAsync(), _ => SelectedCustomer != null);
    }

    public event Action? NewCustomer;
    public event Action<Customer>? EditCustomer;

    public async Task LoadCustomersAsync()
    {
        Customers.Clear();
        foreach (var c in await _repo.GetCustomersAsync())
            Customers.Add(c);
    }

    private async Task DeleteAsync()
    {
        if (SelectedCustomer == null)
            return;
        await _repo.DeleteCustomerAsync(SelectedCustomer.Id);
        await LoadCustomersAsync();
    }
}
