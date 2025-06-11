using System;
using System.Threading.Tasks;
using ProjectControl.Core.Models;
using ProjectControl.Data;
using ProjectControl.Desktop.Commands;

namespace ProjectControl.Desktop.ViewModels;

public class ProjectEditorViewModel
{
    private readonly ProjectRepository _repo;
    public Project Project { get; }

    public DelegateCommand SaveCommand { get; }

    public event Action? Saved;

    public ProjectEditorViewModel(ProjectRepository repo, Project? project = null)
    {
        _repo = repo;
        Project = project ?? new Project();
        SaveCommand = new DelegateCommand(async _ => await SaveAsync());
    }

    private async Task SaveAsync()
    {
        if (Project.Id == 0)
            await _repo.AddProjectAsync(Project);
        else
            await _repo.UpdateProjectAsync(Project);
        Saved?.Invoke();
    }
}
