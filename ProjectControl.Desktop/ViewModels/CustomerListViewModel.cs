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

    public CustomerListViewModel(CustomerRepository repo)
    {
        _repo = repo;
        NewCustomerCommand = new DelegateCommand(_ => NewCustomer?.Invoke());
    }

    public event System.Action? NewCustomer;

    public async Task LoadCustomersAsync()
    {
        Customers.Clear();
        foreach (var c in await _repo.GetCustomersAsync())
            Customers.Add(c);
    }
}
