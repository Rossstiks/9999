using System.Windows;
using ProjectControl.Desktop.ViewModels;

namespace ProjectControl.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
