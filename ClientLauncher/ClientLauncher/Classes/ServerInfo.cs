using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLauncher
{
   [Serializable]
    public class ServerInfo
    {
        
        private Guid guServerId = Guid.Empty;
        private string strServerName = "SWGANH NoName Server";
        private string strServerDescription = "No Description Found";
        private string strAddress = "127.0.0.1";
        private int nPort = 27000;
        private string strRSSFeedUrl = "http://news.rss.com";
        private DateTime dtLastUpdated = new DateTime(1906, 6, 6, 7, 6, 0);
        private int nPopulation = 0;
        private int nCharsCreated = 0;
        private int nLauncherPort = 0;
        private string strSafeFolderName = "";

       
        public Guid ServerId
        {
            get
            {
                return guServerId;
            }
            set
            {
                guServerId = value;
            }
        }
        
        public string ServerName
        {
            get
            {
                return strServerName;
            }
            set
            {
                strServerName = value;
            }
        }
        
        public string Description
        {
            get
            {
                return strServerDescription;
            }
            set
            {
                strServerDescription = value;
            }
        }
        
        public string Address
        {
            get
            {
                return strAddress;
            }
            set
            {
                strAddress = value;
            }
        }
       
        public int Port
        {
            get
            {
                return nPort;
            }
            set
            {
                nPort = value;
            }
        }

        
        public int LauncherPort
        {
            get
            {
                return nLauncherPort;
            }
            set
            {
                nLauncherPort = value;
            }
        }
       
        public string RSSFeedUrl
        {
            get
            {
                return strRSSFeedUrl;
            }
            set
            {
                strRSSFeedUrl = value;
            }
        }
        
        public DateTime LastUpdated
        {
            get
            {
                return dtLastUpdated;
            }
            set
            {
                dtLastUpdated = value;
            }
        }
       
        public int Population
        {
            get
            {
                return nPopulation;
            }
            set
            {
                nPopulation = value;
            }
        }
        
        public int CharsCreated
        {
            get
            {
                return nCharsCreated;
            }
            set
            {
                nCharsCreated = value;
            }
        }

        
        public string SafeFolderName
        {
            get
            {
                strSafeFolderName = "";
                foreach (char theChar in ServerName.ToCharArray())
                {
                    int ascii = (int)theChar;

                    if ((ascii == 32) || ((ascii >= 48) && (ascii <= 57)) || ((ascii >= 65) && (ascii <= 90)) || ((ascii >= 97) && (ascii <= 122)))
                    {
                        strSafeFolderName += theChar;
                    }
                }
                return strSafeFolderName;
            }
            set
            {
                strSafeFolderName = "";
            }
        }
    }
}
