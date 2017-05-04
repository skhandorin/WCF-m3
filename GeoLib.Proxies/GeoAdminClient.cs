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
    public class GeoAdminClient : DuplexClientBase<GeoLib.Contracts.IGeoAdminService>, IGeoAdminService
    {
        public GeoAdminClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        { }

        public GeoAdminClient(InstanceContext callbackInstance, string endpointName)
            : base(callbackInstance, endpointName)
        { }

        public GeoAdminClient(InstanceContext callbackInstance, Binding binding, EndpointAddress address)
            : base(callbackInstance, binding, address)
        { }



        public void UpdateZipCity(string zip, string city)
        {
            Channel.UpdateZipCity(zip, city);
        }

        public int UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
        {
            return Channel.UpdateZipCity(zipCityData);
        }

        //public Task<int> UpdateZipCityAsync(IEnumerable<ZipCityData> zipCityData)
        //{
        //    return Channel.UpdateZipCityAsync(zipCityData);
        //}
    }
}
