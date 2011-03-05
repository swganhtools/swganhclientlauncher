using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ClientLauncher
{
    public class ServiceMaker
    {
        public ServiceMaker()
        {

        }

        public LauncherData.LauncherDataClient GetServiceClient()
        {
            BasicHttpBinding myBinding = new BasicHttpBinding();
            myBinding.Name = "BasicHttpBinding_ILauncherData";
            myBinding.CloseTimeout = new TimeSpan(0, 1, 0);
            myBinding.OpenTimeout = new TimeSpan(0, 1, 0);
            myBinding.SendTimeout = new TimeSpan(0, 1, 0);
            myBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            myBinding.AllowCookies = false;
            myBinding.BypassProxyOnLocal = false;
            myBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            myBinding.MaxBufferSize = 65536 * 1000;
            myBinding.MaxBufferPoolSize = 524288 * 1000;
            myBinding.MaxReceivedMessageSize = 65536 * 1000;
            myBinding.MessageEncoding = WSMessageEncoding.Text;
            myBinding.TextEncoding = Encoding.UTF8;
            myBinding.TransferMode = TransferMode.Buffered;
            myBinding.UseDefaultWebProxy = true;

            myBinding.ReaderQuotas.MaxDepth = 32;
            myBinding.ReaderQuotas.MaxStringContentLength = 8192;
            myBinding.ReaderQuotas.MaxArrayLength = 65536 * 1000;
            myBinding.ReaderQuotas.MaxBytesPerRead = 4096;
            myBinding.ReaderQuotas.MaxNameTableCharCount = 16384;

            myBinding.Security.Mode = BasicHttpSecurityMode.None;

            EndpointAddress myAddress = new EndpointAddress("http://swganh.hooni.us/LauncherData.svc");
            return new ClientLauncher.LauncherData.LauncherDataClient(myBinding, myAddress);
        }

    }
}
