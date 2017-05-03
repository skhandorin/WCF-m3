using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Contracts
{
    [ServiceContract(CallbackContract = typeof(IUpdateZipCallback))]
    public interface IGeoServiceAdmin
    {
        [OperationContract(Name = "UpdateZipCity_One")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateZipCity(string zip, string city);

        [OperationContract(Name = "UpdateZipCityBatch")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        int UpdateZipCity(IEnumerable<ZipCityData> zipCityData);
    }

    [ServiceContract]
    public interface IUpdateZipCallback
    {
        [OperationContract(IsOneWay = false)]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ZipUpdated(ZipCityData zipCityData);
    }
}
