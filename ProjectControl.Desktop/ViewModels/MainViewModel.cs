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

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ProjectRepository _repo;
    private Project? _activeProject;

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
            OnPropertyChanged(nameof(SelectedProject));
        }
    }

    public DelegateCommand PlayCommand { get; }
    public DelegateCommand PauseCommand { get; }
    public DelegateCommand StopCommand { get; }
    public DelegateCommand NewProjectCommand { get; }

    private bool _completedOnly;
    public bool CompletedOnly
    {
        get => _completedOnly;
        set
        {
            if (_completedOnly != value)
            {
                _completedOnly = value;
                OnPropertyChanged(nameof(CompletedOnly));
            }
        }
    }

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private DateTime _now = DateTime.Now;
    public DateTime Now
    {
        get => _now;
        private set
        {
            _now = value;
            OnPropertyChanged(nameof(Now));
        }
    }

    public MainViewModel(ProjectRepository repo)
    {
        _repo = repo;
        PlayCommand = new DelegateCommand(async _ => await StartTimerAsync(), _ => SelectedProject != null);
        PauseCommand = new DelegateCommand(async _ => await PauseTimerAsync(), _ => SelectedProject != null);
        StopCommand = new DelegateCommand(async _ => await StopTimerAsync(), _ => SelectedProject != null);
        NewProjectCommand = new DelegateCommand(_ => NewProject?.Invoke());
        _timer.Tick += (_, _) => Now = DateTime.Now;
        _timer.Start();
    }

    public Func<Task>? LoadProjectsAsyncAction { get; set; }
    public Action? NewProject { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public async Task LoadProjectsAsync()
    {
        Projects.Clear();
        var all = await _repo.GetProjectsWithCustomerAsync(CompletedOnly ? ProjectStatus.Completed : null);
        foreach (var p in all)
        {
            if (!CompletedOnly && p.Status == ProjectStatus.Completed)
                continue;
            Projects.Add(p);
        }
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
        _activeProject = SelectedProject;
        await LoadProjectsAsync();
    }

    private async Task PauseTimerAsync()
    {
        if (SelectedProject == null)
            return;

        await _repo.PauseTimerAsync(SelectedProject.Id);
        if (_activeProject?.Id == SelectedProject.Id)
            _activeProject = null;
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

        if (_activeProject?.Id == SelectedProject.Id)
            _activeProject = null;

        await LoadProjectsAsync();
    }
}
