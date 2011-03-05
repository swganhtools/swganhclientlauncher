using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LauncherData
{
    #region Service Methods
    // NOTE: If you change the interface name "ILauncherData" here, you must also update the reference to "ILauncherData" in Web.config.
    [ServiceContract]
    public interface ILauncherData
    {
        [OperationContract]
        List<ServerInfo> GetServers();

        [OperationContract]
        void UpdateServer(ServerInfo theServerInfo);

        [OperationContract]
        List<LauncherVersion> GetLatestVersion(string strCurrentVersion);

        [OperationContract]
        LatestFile GetLatestExecutable();

        [OperationContract]
        List<CustomTre> GetCustomTre(Guid guServerId);

        [OperationContract]
        List<StandardTre> GetStandardTre();
    }
    #endregion

    #region Data Objects
    [DataContract]
    public class LatestFile
    {
        private string strFileName;
        private byte[] arFileBytes;

        [DataMember]
        public string FileName
        {
            get
            {
                return strFileName;
            }
            set
            {
                strFileName = value;
            }
        }

        [DataMember]
        public byte[] FileBytes
        {
            get
            {
                return arFileBytes;
            }
            set
            {
                arFileBytes = value;
            }
        }
    }

    [DataContract]
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

        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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

        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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

        [DataMember]
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

    [DataContract]
    public class LauncherVersion
    {
        private Guid guVersionId = Guid.Empty;
        private string strVersionId = "0.1";
        private DateTime dtDateCreated = new DateTime(2010, 03, 21, 16, 12, 30);
        private string strPatchNotes = "No Patch Notes Found";
        private string strLocation = "/launcher/SWGANHLauncher.msi";
        private long nFileSize = 0;
        [DataMember]
        public Guid VersionId
        {
            get
            {
                return guVersionId;
            }
            set
            {
                guVersionId = value;
            }
        }
        [DataMember]
        public string VersionNumber
        {
            get
            {
                return strVersionId;
            }
            set
            {
                strVersionId = value;
            }
        }
        [DataMember]
        public DateTime DateCreated
        {
            get
            {
                return dtDateCreated;
            }
            set
            {
                dtDateCreated = value;
            }
        }
        [DataMember]
        public string PatchNotes
        {
            get
            {
                return strPatchNotes;
            }
            set
            {
                strPatchNotes = value;
            }

        }

        [DataMember]
        public string Location
        {
            get
            {
                return strLocation;
            }
            set
            {
                strLocation = value;
            }
        }

        [DataMember]
        public long FileSize
        {
            get
            {
                return nFileSize;
            }
            set
            {
                nFileSize = value;
            }
        }
    }

    [DataContract]
    public class StandardTre
    {
        private string strFilename;
        private string strMD5Hash;
         [DataMember]
        public string Filename
        {
            get
            {
                return strFilename;
            }
            set
            {
                strFilename = value;
            }
        }
         [DataMember]
        public string MD5Hash
        {
            get
            {
                return strMD5Hash;
            }
            set
            {
                strMD5Hash = value;
            }
        }
    }

    [DataContract]
    public class CustomTre
    {
        private string strFileName;
        private string strUri;
        private string strMD5Hash;

        [DataMember]
        public string FileName
        {
            get
            {
                return strFileName;
            }
            set
            {
                strFileName = value;
            }
        }

        [DataMember]
        public string Uri
        {
            get
            {
                return strUri;
            }
            set
            {
                strUri = value;
            }
        }

        [DataMember]
        public string MD5Hash
        {
            get
            {
                return strMD5Hash;
            }
            set
            {
                strMD5Hash = value;
            }
        }
    }

    #endregion
}
