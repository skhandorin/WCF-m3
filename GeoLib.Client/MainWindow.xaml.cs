using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeoLib.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _Proxy = new GeoClient("tcpEP");
            _Proxy.Open();
            _ProxyStateful = new StatefulGeoClient();

            this.Title = "UI Running on Thread " + Thread.CurrentThread.ManagedThreadId +
                " | Process " + Process.GetCurrentProcess().Id.ToString();
        }

        GeoClient _Proxy = null;
        StatefulGeoClient _ProxyStateful = null;

        private async void btnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text != "")
            {
                //GeoClient proxy = new GeoClient("tcpEP");
                GeoClient proxy = _Proxy;
                string zipCode = txtZipCode.Text;

                try
                {
                    ZipCodeData data = await Task.Run(() => proxy.GetZipInfo(zipCode));
                    if (data != null)
                    {
                        lblCity.Content = data.City;
                        lblState.Content = data.State;
                    }
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    MessageBox.Show("Exception thrown by service.\n\rException type: " +
                        "FaultException<ExceptionDetail>\r\n" +
                        "Message: " + ex.Message + "\r\n" +
                        "Detailed message: " + ex.Detail.Message + "\r\n" +
                        "Proxy state: " + proxy.State.ToString());
                }
                catch (FaultException ex)
                {
                    MessageBox.Show("FaultException thrown by service.\n\rException type: " +
                        ex.GetType().Name + "\n\r" +
                        "Message: " + ex.Message + "\r\n" +
                        "Proxy state: " + proxy.State.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception thrown by service.\n\rException type: " + 
                        ex.GetType().Name + "\n\r" +
                        "Message: " + ex.Message + "\r\n" +
                        "Proxy state: " + proxy.State.ToString());
                }

                if (proxy != _Proxy)
                {
                    proxy.Close();
                }
            }
        }

        private void btnGetZipCodes_Click(object sender, RoutedEventArgs e)
        {
            if (txtState.Text != "")
            {
                //EndpointAddress address = new EndpointAddress("net.tcp://localhost:8009/GeoService");
                //Binding binding = new NetTcpBinding();

                //GeoClient proxy = new GeoClient(binding, address);
                //GeoClient proxy = new GeoClient("tcpEP");

                IEnumerable<ZipCodeData> data = _Proxy.GetZips(txtState.Text);
                if (data != null)
                {
                    lstZips.ItemsSource = data;
                }

                //proxy.Close();
            }
        }

        private void btnMakeCall_Click(object sender, RoutedEventArgs e)
        {
            EndpointAddress address = new EndpointAddress("net.tcp://localhost:8010/MessageService");
            Binding binding = new NetTcpBinding();

            //ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>("");
            ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>(binding, address);
            IMessageService proxy = factory.CreateChannel();

            proxy.ShowMsg(txtMessage.Text);

            factory.Close();
        }

        private void btnPush_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text != "")
            {
                _ProxyStateful.PushZip(txtZipCode.Text);
            }
        }

        private void bthGetPushedInfo_Click(object sender, RoutedEventArgs e)
        {
            ZipCodeData data = _ProxyStateful.GetZipInfo();

            if (data != null)
            {
                lblCity.Content = data.City;
                lblState.Content = data.State;
            }
        }

        private void btnGetInRange_Click(object sender, RoutedEventArgs e)
        {
            if (txtRange.Text != "")
            {
                IEnumerable<ZipCodeData> data = _ProxyStateful.GetZips(int.Parse(txtRange.Text));
                if (data != null)
                {
                    lstZips.ItemsSource = data;
                }
            }
        }
    }
}
