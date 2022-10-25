using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BRAGI.Util;
using BRAGI.Valhalla;
using ControlzEx.Standard;
using MahApps.Metro.Controls;

namespace BRAGI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.SetOut(new ControlWriter(ConsoleBox));
            Valhalla.Valhalla v = null;
            try
            {
                v = Valhalla.Valhalla.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ValhallaOpenedSinceTxt.Text = "Error during opening of Valhalla Gates";
            }
            if (v != null)
            {
                ValhallaStatusToggle.DataContext = v;
                ValhallaOpenedSinceTxt.DataContext = v;
                ValhallaHeroToggle.DataContext = v;
                ValhallaHeroArrivedTxt.DataContext = v;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            ConsoleBox.Text = "";
        }
    }
}
