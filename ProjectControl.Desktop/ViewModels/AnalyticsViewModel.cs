using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ProjectControl.Core.Models;
using ProjectControl.Data;

namespace ProjectControl.Desktop.ViewModels;

public class AnalyticsViewModel
{
    private readonly ProjectRepository _repo;
    private readonly CustomerRepository _customerRepo;
    public ObservableCollection<Project> Projects { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public bool CompletedOnly { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Customer? SelectedCustomer { get; set; }
    public string? AmountFrom { get; set; }
    public string? AmountTo { get; set; }

    public AnalyticsViewModel(ProjectRepository repo, CustomerRepository customerRepo)
    {
        _repo = repo;
        _customerRepo = customerRepo;
    }

    public long TotalTimeSpent => Projects.Sum(p => p.TotalTimeSpent);
    public double TotalPayment => Projects.Sum(p => p.PaymentAmount ?? 0);

    public async Task LoadProjectsAsync()
    {
        Projects.Clear();
        var status = CompletedOnly ? ProjectStatus.Completed : null as ProjectStatus?;

        double? minAmount = double.TryParse(AmountFrom, out var min) ? min : null;
        double? maxAmount = double.TryParse(AmountTo, out var max) ? max : null;

        var list = await _repo.GetProjectsWithCustomerAsync(
            status,
            FromDate,
            ToDate,
            SelectedCustomer?.Id,
            minAmount,
            maxAmount);

        foreach (var p in list)
            Projects.Add(p);
    }

    public async Task LoadCustomersAsync()
    {
        Customers.Clear();
        foreach (var c in await _customerRepo.GetCustomersAsync())
            Customers.Add(c);
    }

    public void ExportCsv(string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        writer.WriteLine("Project,Customer,Time,Payment");
        foreach (var p in Projects)
        {
            string time = TimeSpan.FromSeconds(p.TotalTimeSpent).ToString(@"hh\:mm\:ss");
            string pay = (p.PaymentAmount ?? 0).ToString(CultureInfo.InvariantCulture);
            writer.WriteLine($"{Escape(p.Name)},{Escape(p.Customer?.Name ?? string.Empty)},{time},{pay}");
        }
    }

    private static string Escape(string value)
    {
        if (value.Contains('"'))
            value = value.Replace("\"", "\"\"");
        return value.Contains(',') ? $"\"{value}\"" : value;
    }
}
