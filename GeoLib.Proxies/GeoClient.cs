using GeoLib.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Proxies
{
    public class GeoClient : DuplexClientBase<IGeoService>, GeoLib.Proxies.IGeoService
    {
        public GeoClient(InstanceContext callbackInstance) 
            : base(callbackInstance)
        { }

        public GeoClient(InstanceContext callbackInstance, string endpointName) 
            : base(callbackInstance, endpointName)
        { }

        public GeoClient(InstanceContext callbackInstance, Binding binding, EndpointAddress address) 
            : base(callbackInstance, binding, address)
        { }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            return Channel.GetStates(primaryOnly);
        }

        public ZipCodeData GetZipInfo(string zip)
        {
            return Channel.GetZipInfo(zip);
        }

        public IEnumerable<ZipCodeData> GetZips(string state)
        {
            return Channel.GetZips(state);
        }

        public IEnumerable<ZipCodeData> GetZips(string zip, int range)
        {
            return Channel.GetZips(zip, range);
        }

        public void OneWayExample()
        {
            Channel.OneWayExample();
        }

        public void UpdateZipCity(string zip, string city)
        {
            Channel.UpdateZipCity(zip, city);
        }

        public int UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
        {
            return Channel.UpdateZipCity(zipCityData);
        }

        public Task<int> UpdateZipCityAsync(IEnumerable<ZipCityData> zipCityData)
        {
            return Channel.UpdateZipCityAsync(zipCityData);
        }
    }
}
