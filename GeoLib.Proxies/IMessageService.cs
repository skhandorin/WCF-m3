﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Client.Contracts
{
    [ServiceContract(Namespace = "http://www.pluralsight.com/MiguelCastro/WcfEndToEnd")]
    public interface IMessageService
    {
        [OperationContract]
        void ShowMessage(string message);
    }
}
