using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectControl.Desktop.Converters;

public class TimeFormatConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is long seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return ts.ToString(@"hh\:mm\:ss");
        }
        return value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

