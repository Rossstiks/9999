using System.Windows;
using Microsoft.Win32;
using ProjectControl.Desktop.ViewModels;

namespace ProjectControl.Desktop.Views;

public partial class AnalyticsWindow : Window
{
    public AnalyticsWindow(AnalyticsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private async void OnFilterChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AnalyticsViewModel vm)
            await vm.LoadProjectsAsync();
    }

    private void OnExportCsv(object sender, RoutedEventArgs e)
    {
        if (DataContext is not AnalyticsViewModel vm)
            return;
        var dlg = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            FileName = "projects.csv"
        };
        if (dlg.ShowDialog() == true)
            vm.ExportCsv(dlg.FileName);
    }
}
