using GeoLib.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Proxies
{
    public class GeoClient : ClientBase<IGeoService>, IGeoService
    {
        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            return Channel
        }

        public ZipCodeData GetZipInfo(string zip)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ZipCodeData> GetZips(string state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ZipCodeData> GetZips(string zip, int range)
        {
            throw new NotImplementedException();
        }
    }
}
