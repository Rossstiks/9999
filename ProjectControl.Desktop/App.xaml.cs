using System.Windows;

namespace ProjectControl.Desktop;

public partial class App : Application
{
    public void ApplyTheme(string theme)
    {
        Resources.MergedDictionaries.Clear();
        var dict = new ResourceDictionary
        {
            Source = new System.Uri($"Themes/{theme}Theme.xaml", System.UriKind.Relative)
        };
        Resources.MergedDictionaries.Add(dict);
    }
}
