using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using ProjectControl.Core.Models;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public class MainViewModel
{
    private readonly ProjectRepository _repo;
    private Project? _activeProject;

    public ObservableCollection<Project> Projects { get; } = new();
    public ObservableCollection<TimeEntry> TimeEntries { get; } = new();

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
    }

    public Func<Task>? LoadProjectsAsyncAction { get; set; }
    public Action? NewProject { get; set; }

    public async Task LoadProjectsAsync()
    {
        Projects.Clear();
        foreach (var p in await _repo.GetProjectsAsync())
            Projects.Add(p);
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
