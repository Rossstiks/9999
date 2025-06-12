using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using ProjectControl.Core.Models;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public enum ProjectSortMode
{
    Name,
    Time
}

public class MainViewModel
{
    private readonly ProjectRepository _repo;
    private Project? _activeProject;
    private readonly DispatcherTimer _timer;

    public ObservableCollection<Project> Projects { get; } = new();
    public ObservableCollection<Project> FilteredProjects { get; } = new();
    public ObservableCollection<TimeEntry> TimeEntries { get; } = new();

    public string FilterText { get; set; } = string.Empty;
    public ProjectSortMode SortMode { get; set; } = ProjectSortMode.Name;

    private Project? _selectedProject;
    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            PlayCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }
    }

    public DelegateCommand PlayCommand { get; }
    public DelegateCommand PauseCommand { get; }
    public DelegateCommand StopCommand { get; }
    public DelegateCommand NewProjectCommand { get; }

    public MainViewModel(ProjectRepository repo)
    {
        _repo = repo;
        PlayCommand = new DelegateCommand(async _ => await StartTimerAsync(), _ => SelectedProject != null);
        PauseCommand = new DelegateCommand(async _ => await PauseTimerAsync(), _ => SelectedProject != null);
        StopCommand = new DelegateCommand(async _ => await StopTimerAsync(), _ => SelectedProject != null);
        NewProjectCommand = new DelegateCommand(_ => NewProject?.Invoke());

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => _activeProject?.NotifyRunningTimeChanged();
        _timer.Start();
    }

    public Func<Task>? LoadProjectsAsyncAction { get; set; }
    public Action? NewProject { get; set; }

    public async Task LoadProjectsAsync()
    {
        Projects.Clear();
        foreach (var p in await _repo.GetProjectsWithCustomerAsync())
            Projects.Add(p);
        _activeProject = Projects.FirstOrDefault(p => p.CurrentTimerStartTime != null);
        ApplyFilterSort();
    }

    public void ApplyFilterSort()
    {
        var query = Projects.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(FilterText))
            query = query.Where(p => p.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        query = SortMode switch
        {
            ProjectSortMode.Time => query.OrderByDescending(p => p.TotalTimeSpent),
            _ => query.OrderBy(p => p.Name)
        };
        FilteredProjects.Clear();
        foreach (var p in query)
            FilteredProjects.Add(p);
    }

    private async Task StartTimerAsync()
    {
        if (SelectedProject == null)
            return;

        if (_activeProject != null && _activeProject.Id != SelectedProject.Id)
            await _repo.PauseTimerAsync(_activeProject.Id);

        await _repo.StartTimerAsync(SelectedProject.Id);
        await LoadProjectsAsync();
        _activeProject?.NotifyRunningTimeChanged();
    }

    private async Task PauseTimerAsync()
    {
        if (SelectedProject == null)
            return;

        await _repo.PauseTimerAsync(SelectedProject.Id);
        await LoadProjectsAsync();
    }

    private async Task StopTimerAsync()
    {
        if (SelectedProject == null)
            return;

        await _repo.CompleteProjectAsync(
            SelectedProject.Id,
            PaymentStatus.Unpaid,
            0,
            null,
            null);

        await LoadProjectsAsync();
    }
}
