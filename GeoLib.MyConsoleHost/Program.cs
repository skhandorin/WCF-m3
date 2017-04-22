﻿using GeoLib.Contracts;
using GeoLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.MyConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost hostGeoManager = new ServiceHost(typeof(GeoManager));

            ProceduralBinding(hostGeoManager);

            hostGeoManager.Open();

            Console.WriteLine("Services started. Press [Enter] to exit.");
            Console.ReadLine();

            hostGeoManager.Close();
        }

        private static void ProceduralBinding(ServiceHost hostGeoManager)
        {
            string address = "net.tcp://localhost:8009/GeoService";
            Binding binding = new NetTcpBinding();
            Type contract = typeof(IGeoService);

            

            string address_http = "http://localhost:8081/GeoService";
            Binding binding_http = new BasicHttpBinding();
            Type contract_http = typeof(IGeoService);

            hostGeoManager.AddServiceEndpoint(contract_http, binding_http, address_http);
            hostGeoManager.AddServiceEndpoint(contract, binding, address);
        }
    }
}