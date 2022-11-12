using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Win32;
using BRAGI.UI.ViewModels;
using BRAGI.Util;
using BRAGI.Util.AudioUtil;
using BRAGI.Valhalla;
using System;
using System.ComponentModel;
using System.Diagnostics;

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
        KeyInput.InitializeHook();
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

    private void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        Console.WriteLine("Key Down: {0}", e.Key);
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