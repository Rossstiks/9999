using System.Windows;
using ProjectControl.Desktop.ViewModels;

namespace ProjectControl.Desktop.Views;

public partial class ProjectEditorWindow : Window
{
    public ProjectEditorWindow(ProjectEditorViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
