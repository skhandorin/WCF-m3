using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib.WpfHost.Contracts;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace GeoLib.WpfHost.Services
{
    [ServiceBehavior(UseSynchronizationContext = false)]
    public class MessageManager : IMessageService
    {
        public void ShowMessage(string message)
        {
            MainWindow.MainUI.ShowMessage(message + 
                " Thread " + Thread.CurrentThread.ManagedThreadId.ToString() +
                " | Process " + Process.GetCurrentProcess().Id.ToString());
        }
    }
}
