using ProjectControl.Core.Models;
using ProjectControl.Data;
using ProjectControl.Desktop.Commands;
using System;
using System.Threading.Tasks;

namespace ProjectControl.Desktop.ViewModels;

public class CustomerEditorViewModel
{
    private readonly CustomerRepository _repo;
    public Customer Customer { get; }

    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DeleteCommand { get; }

    public event Action? Saved;
    public event Action? Deleted;

    public CustomerEditorViewModel(CustomerRepository repo, Customer? customer = null)
    {
        _repo = repo;
        Customer = customer ?? new Customer();
        SaveCommand = new DelegateCommand(async _ => await SaveAsync());
        DeleteCommand = new DelegateCommand(async _ => await DeleteAsync(), _ => Customer.Id != 0);
    }

    private async Task SaveAsync()
    {
        if (Customer.Id == 0)
            await _repo.AddCustomerAsync(Customer);
        else
            await _repo.UpdateCustomerAsync(Customer);
        Saved?.Invoke();
    }

    private async Task DeleteAsync()
    {
        if (Customer.Id == 0) return;
        await _repo.DeleteCustomerAsync(Customer.Id);
        Deleted?.Invoke();
    }
}
