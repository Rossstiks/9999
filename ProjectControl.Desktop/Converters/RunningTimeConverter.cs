using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectControl.Desktop.Converters;

public class RunningTimeConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 3)
            return null;
        if (values[0] is long total && values[2] is DateTime now)
        {
            long extra = 0;
            if (values[1] is DateTime start)
                extra = (long)(now - start).TotalSeconds;
            var ts = TimeSpan.FromSeconds(total + extra);
            return ts.ToString(@"hh\:mm\:ss");
        }
        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
