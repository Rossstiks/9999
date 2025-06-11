using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectControl.Core.Models;

namespace ProjectControl.Data;

public class CustomerRepository
{
    private readonly ProjectControlContext _context;

    public CustomerRepository(ProjectControlContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetCustomersAsync()
        => await _context.Customers.ToListAsync();

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> GetCustomerAsync(long id)
        => await _context.Customers.FindAsync(id);

    public async Task UpdateCustomerAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCustomerAsync(long id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return;
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }
}
