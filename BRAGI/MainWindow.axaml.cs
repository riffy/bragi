using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using BRAGI.UI.ViewModels;
using BRAGI.Util;
using BRAGI.Valhalla;
using System;
using System.ComponentModel;

namespace BRAGI;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }
    private MainWindowViewModel viewModel { get; set; }
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        Console.SetOut(new ControlWriter(ConsoleBox));
        Audio.RegisterAudioEvents();
        viewModel = new();
        DataContext = viewModel;
        try
        {
            Valhalla.Valhalla v = Valhalla.Valhalla.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void ToggleTest_Click(object sender, RoutedEventArgs e)
    {
        viewModel.ToggleTest();
    }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        viewModel.SaveSettings();
    }

    private void CancelSettings_Click(object sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(delegate
        {
            viewModel.ReadAudioAndUpdate();
        });
    }

    private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
    {
        ConsoleBox.Clear();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Environment.Exit(0);
    }
}