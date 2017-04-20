using GeoLib.Services;
using GeoLib.WpfHost.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

namespace GeoLib.WpfHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MainUI { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;

            MainUI = this;

            this.Title = "UI Running on Thread " + Thread.CurrentThread.ManagedThreadId +
                " | Process " + Process.GetCurrentProcess().Id.ToString();
        }

        ServiceHost _HostGeoManager = null;
        ServiceHost _HostMessageManager = null;

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _HostGeoManager = new ServiceHost(typeof(GeoManager));
            _HostMessageManager = new ServiceHost(typeof(MessageManager));

            _HostGeoManager.Open();
            _HostMessageManager.Open();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _HostGeoManager.Close();
            _HostMessageManager.Close();

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        public void ShowMessage(string message)
        {
            int ThreadId = Thread.CurrentThread.ManagedThreadId;

            lblMessage.Content = message + Environment.NewLine +
                "shown on Thread " + Thread.CurrentThread.ManagedThreadId +
                " | Process " + Process.GetCurrentProcess().Id.ToString();
        }
    }
}
