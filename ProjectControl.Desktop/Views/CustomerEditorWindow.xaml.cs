using System.Windows;
using ProjectControl.Desktop.ViewModels;

namespace ProjectControl.Desktop.Views;

public partial class CustomerEditorWindow : Window
{
    public CustomerEditorWindow(CustomerEditorViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
