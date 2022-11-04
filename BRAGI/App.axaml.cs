using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentAvalonia.Styling;

namespace BRAGI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        var faTheme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
        if (faTheme != null) faTheme.RequestedTheme = "Dark";

        base.OnFrameworkInitializationCompleted();
    }
}