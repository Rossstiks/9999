using System.Windows;
using ProjectControl.Desktop.ViewModels;

namespace ProjectControl.Desktop.Views;

public partial class CustomerListWindow : Window
{
    public CustomerListWindow(CustomerListViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
