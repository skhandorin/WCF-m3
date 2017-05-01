﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Contracts
{
    [ServiceContract]
    public interface IGeoService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationException))]
        [FaultContract(typeof(NotFoundData))]
        ZipCodeData GetZipInfo(string zip);

        [OperationContract]
        IEnumerable<string> GetStates(bool primaryOnly);

        [OperationContract(Name = "GetZipsByState")]
        IEnumerable<ZipCodeData> GetZips(string state);

        [OperationContract(Name = "GetZipsForRange")]
        IEnumerable<ZipCodeData> GetZips(string zip, int range);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateZipCity(string zip, string city);

        [OperationContract(Name = "UpdateZipCityBatch")]
        [TransactionFlow(TransactionFlowOption.NotAllowed)]
        void UpdateZipCity(IEnumerable<ZipCityData> zipCityData);

        [OperationContract(IsOneWay = true)]
        void OneWayExample();
    }
}
