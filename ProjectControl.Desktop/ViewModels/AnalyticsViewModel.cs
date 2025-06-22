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
    public ObservableCollection<Project> Projects { get; } = new();
    public bool CompletedOnly { get; set; }

    public AnalyticsViewModel(ProjectRepository repo)
    {
        _repo = repo;
    }

    public long TotalTimeSpent => Projects.Sum(p => p.TotalTimeSpent);
    public double TotalPayment => Projects.Sum(p => p.PaymentAmount ?? 0);

    public async Task LoadProjectsAsync()
    {
        Projects.Clear();
        var status = CompletedOnly ? ProjectStatus.Completed : null as ProjectStatus?;
        foreach (var p in await _repo.GetProjectsWithCustomerAsync(status))
            Projects.Add(p);
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
