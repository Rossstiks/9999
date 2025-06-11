using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using ProjectControl.Core.Models;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public class MainViewModel
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private DateTime _currentStart;
    private Project? _activeProject;

    public ObservableCollection<Project> Projects { get; } = new();
    public ObservableCollection<TimeEntry> TimeEntries { get; } = new();

    public DelegateCommand PlayCommand { get; }
    public DelegateCommand PauseCommand { get; }
    public DelegateCommand StopCommand { get; }

    public MainViewModel()
    {
        _timer.Tick += (_, _) => { /* tick placeholder */ };
        PlayCommand = new DelegateCommand(_ => StartTimer());
        PauseCommand = new DelegateCommand(_ => PauseTimer(), _ => _activeProject != null);
        StopCommand = new DelegateCommand(_ => StopTimer(), _ => _activeProject != null);

        // Demo data
        Projects.Add(new Project { Id = 1, Name = "Первый проект", CustomerId = 1 });
        Projects.Add(new Project { Id = 2, Name = "Второй проект", CustomerId = 1 });
    }

    private void StartTimer()
    {
        if (_activeProject != null)
        {
            PauseTimer();
        }
        _activeProject = Projects.FirstOrDefault();
        if (_activeProject == null) return;
        _currentStart = DateTime.Now;
        _activeProject.CurrentTimerStartTime = _currentStart;
        _activeProject.Status = ProjectStatus.InProgress;
        _timer.Start();
    }

    private void PauseTimer()
    {
        if (_activeProject == null) return;
        _timer.Stop();
        var end = DateTime.Now;
        var duration = (long)(end - _currentStart).TotalSeconds;
        TimeEntries.Add(new TimeEntry
        {
            ProjectId = _activeProject.Id,
            StartTime = _currentStart,
            EndTime = end,
            Duration = duration
        });
        _activeProject.TotalTimeSpent += duration;
        _activeProject.CurrentTimerStartTime = null;
        _activeProject.Status = ProjectStatus.Paused;
        _activeProject = null;
    }

    private void StopTimer()
    {
        if (_activeProject == null) return;
        var project = _activeProject;
        PauseTimer();
        project.ActualCompletionDate = DateTime.Now;
        project.Status = ProjectStatus.Completed;
    }
}
