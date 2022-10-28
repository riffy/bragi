using Avalonia.Controls;
using Avalonia.Interactivity;
using BRAGI.Util;
using BRAGI.Valhalla;
using System;
using System.ComponentModel;

namespace BRAGI;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        Console.SetOut(new ControlWriter(ConsoleBox));
        try
        {
            Valhalla.Valhalla v = Valhalla.Valhalla.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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