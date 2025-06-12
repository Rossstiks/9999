using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectControl.Core.Models;

namespace ProjectControl.Data;

public class ProjectRepository
{
    private readonly ProjectControlContext _context;

    public ProjectRepository(ProjectControlContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetProjectsAsync()
        => await _context.Projects.ToListAsync();

    public async Task<List<Project>> GetProjectsWithCustomerAsync(ProjectStatus? status = null)
    {
        var query = _context.Projects.Include(p => p.Customer).AsQueryable();
        if (status != null)
            query = query.Where(p => p.Status == status);
        return await query.ToListAsync();
    }

    public async Task<Project> AddProjectAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task StartTimerAsync(long projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return;
        if (project.CurrentTimerStartTime != null)
            await PauseTimerAsync(projectId);
        project.CurrentTimerStartTime = DateTime.Now;
        project.Status = ProjectStatus.InProgress;
        await _context.SaveChangesAsync();
    }

    public async Task PauseTimerAsync(long projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null || project.CurrentTimerStartTime == null) return;
        var start = project.CurrentTimerStartTime.Value;
        var end = DateTime.Now;
        long duration = (long)(end - start).TotalSeconds;
        var entry = new TimeEntry
        {
            ProjectId = project.Id,
            StartTime = start,
            EndTime = end,
            Duration = duration
        };
        _context.TimeEntries.Add(entry);
        project.TotalTimeSpent += duration;
        project.CurrentTimerStartTime = null;
        project.Status = ProjectStatus.Paused;
        await _context.SaveChangesAsync();
    }

    public async Task CompleteProjectAsync(long projectId, PaymentStatus paymentStatus, double amount, DateTime? paymentDate, string? notes)
    {
        await PauseTimerAsync(projectId);
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return;
        project.Status = ProjectStatus.Completed;
        project.ActualCompletionDate = DateTime.Now;
        project.PaymentStatus = paymentStatus;
        project.PaymentAmount = amount;
        project.PaymentDate = paymentDate;
        project.PaymentNotes = notes;
        await _context.SaveChangesAsync();
    }
}
