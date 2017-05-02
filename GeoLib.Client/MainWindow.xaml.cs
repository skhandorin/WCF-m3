﻿using GeoLib.Client.Contracts;
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
using System.Transactions;
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
    [CallbackBehavior(UseSynchronizationContext = false)]
    public partial class MainWindow : Window, IUpdateZipCallback
    {
        public MainWindow()
        {
            InitializeComponent();

            _Proxy = new GeoClient(new InstanceContext(this), "tcpEP");
            _Proxy.Open();
            _ProxyStateful = new StatefulGeoClient();

            this.Title = "UI Running on Thread " + Thread.CurrentThread.ManagedThreadId +
                " | Process " + Process.GetCurrentProcess().Id.ToString();
        }

        GeoClient _Proxy = null;
        StatefulGeoClient _ProxyStateful = null;

        private async void btnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text == "")
            {
                txtZipCode.Text = "07035";
            }

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
                        ex.GetType().Name + "\n\r" +
                        "Message: " + ex.Message + "\r\n" +
                        "Detailed message: " + ex.Detail.Message + "\r\n" +
                        "Proxy state: " + proxy.State.ToString());
                }
                catch (FaultException<NotFoundData> ex)
                {
                    MessageBox.Show("FaultException<NotFoundData> thrown by service.\n\rException type: " +
                        ex.GetType().Name + "\n\r" +
                        "Message: " + ex.Message + "\r\n" +
                        "Detailed message: " + ex.Detail.Message + "\r\n" +
                        "When: " + ex.Detail.When + "\r\n" +
                        "User: " + ex.Detail.User + "\r\n" +
                        "Proxy state: " + proxy.State.ToString());
                }
                catch (FaultException<ApplicationException> ex)
                {
                    MessageBox.Show("FaultException<ApplicationException> thrown by service.\n\rException type: " +
                        ex.GetType().Name + "\n\r" +
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

        private void btnUpdateBatch_Click(object sender, RoutedEventArgs e)
        {
            List<ZipCityData> cityZipList = new List<ZipCityData>()
            {
                new ZipCityData() { ZipCode = "07035", City = "Bedrock" },
                new ZipCityData() { ZipCode = "33030", City = "End of the World" },
                new ZipCityData() { ZipCode = "90210", City = "Alderan" },
                new ZipCityData() { ZipCode = "07094", City = "Storybrooke" }
            };

            try
            {
                GeoClient proxy = new GeoClient(new InstanceContext(this), "tcpEP");
                using (TransactionScope scope = new TransactionScope())
                {
                    proxy.UpdateZipCity(cityZipList);
                    proxy.Close();

                    //throw new ApplicationException("uh oh");

                    scope.Complete();
                }
                
                
                MessageBox.Show("Updated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void ZipUpdated(ZipCityData zipCityData)
        {
            MessageBox.Show($"Updated zipcode {zipCityData.ZipCode} with city {zipCityData.City}");
        }

        private void btnPutBack_Click(object sender, RoutedEventArgs e)
        {
            List<ZipCityData> cityZipList = new List<ZipCityData>()
            {
                new ZipCityData() { ZipCode = "07035", City = "Linkoln Park" },
                new ZipCityData() { ZipCode = "33030", City = "Homestead" },
                new ZipCityData() { ZipCode = "90210", City = "90210-city" },
                new ZipCityData() { ZipCode = "07094", City = "07094-city" }
            };

            try
            {
                GeoClient proxy = new GeoClient(new InstanceContext(this), "tcpEP");
                proxy.UpdateZipCity(cityZipList);
                proxy.Close();
                MessageBox.Show("Updated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnOneWay_Click(object sender, RoutedEventArgs e)
        {
            GeoClient proxy = new GeoClient(new InstanceContext(this), "tcpEP");

            proxy.OneWayExample();

            MessageBox.Show("Oneway example called. Back at client.");

            proxy.Close();

            MessageBox.Show("Proxy is now close.");
        }
    }
}
