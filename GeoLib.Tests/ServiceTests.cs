using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel.Channels;
using GeoLib.Services;
using System.ServiceModel;
using GeoLib.Contracts;

namespace GeoLib.Tests
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod]
        public void test_zip_code_retrieval_db()
        {
            string address = "net.pipe://localhost/GeoService";
            Binding binding = new NetNamedPipeBinding();

            ServiceHost host = new ServiceHost(typeof(GeoManager));

            host.AddServiceEndpoint(typeof(IGeoService), binding, address);
            host.Open();

            ChannelFactory<IGeoService> factory = new ChannelFactory<IGeoService>(binding, new EndpointAddress(address));
            IGeoService proxy = factory.CreateChannel();

            ZipCodeData data = proxy.GetZipInfo("07035");

            Assert.AreEqual(data.City, "Lincoln Park");
            Assert.AreEqual(data.State, "NJ");
        }
    }
}
