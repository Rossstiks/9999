using System;
using System.Windows;

namespace ProjectControl.Desktop.Views;

public partial class PaymentWindow : Window
{
    public double Amount { get; private set; }

    public PaymentWindow()
    {
        InitializeComponent();
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(AmountBox.Text, out var value) && value > 0)
        {
            Amount = value;
            DialogResult = true;
        }
        else
        {
            MessageBox.Show("Введите положительную сумму", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
