using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLauncher
{
    public class ChangeCustomerServerEventArgs : EventArgs
    {
        public bool Delete
        {
            get;
            private set;
        }

        public LauncherData.ServerInfo ServerInfo
        {
            get;
            private set;
        }

        public ChangeCustomerServerEventArgs(bool blnDelete, LauncherData.ServerInfo theServerInfo)
        {
            this.Delete = blnDelete;
            this.ServerInfo = theServerInfo;
        }
    }
}
