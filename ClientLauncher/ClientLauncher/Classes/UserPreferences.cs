using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClientLauncher
{
    public class UserPreferences
    {
        public enum SettingsType
        {
            Skin,
            Pallette,
            GhostFont,
            DefaultServer,
            Locale,
            AddCustomServer,
            RemoveCustomServer,
            AddServerCredentials,
            RemoveServerCredentials,
            ClientPath,
            CachedServers,
            StandardTres
        }

        private UISettings mySettings;
        private UserSettings theSettings;

        public UserPreferences()
        {
            mySettings = new UISettings();
            LoadPreferences();
        }

        private void LoadPreferences()
        {
            BinaryFormatter bf = new BinaryFormatter();
            LauncherEncryption myEncryption = new LauncherEncryption();
            IsolatedStorageFile inFile = IsolatedStorageFile.GetUserStoreForAssembly();
            //do we have the desired locale?
            Locales myLocales = new Locales(CultureInfo.CurrentUICulture.Name);
            
            if (App.Current.Resources["UserPreferences"] == null)
            {
                bool blnSetDefaults = false;
                //here we will attempt to load the settings object from the user's profil
                //or make them up if nothing has been chosen yet (first run)            
                using (IsolatedStorageFileStream inStream = new IsolatedStorageFileStream("settings.cfg", FileMode.OpenOrCreate, System.IO.FileAccess.Read, inFile))
                using (BinaryReader read = new BinaryReader(inStream))
                {

                    if (read.BaseStream.Length == 0)
                    {
                        blnSetDefaults = true;
                    }
                    else
                    {
                        try
                        {
                            //read in the saved bytes
                            byte[] arInBytes = read.ReadBytes((int)read.BaseStream.Length);

                            //decrypt
                            byte[] arDecrypted = myEncryption.Decrypt(arInBytes);

                            //deserialize
                            using (MemoryStream msIn = new MemoryStream(arDecrypted))
                            {
                                theSettings = bf.Deserialize(msIn) as UserSettings;
                            }

                            if (theSettings == null)
                            {
                                //make a default one
                                blnSetDefaults = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            string strYar = ex.Message;
                        }
                    }
                }

                if (blnSetDefaults)
                {
                    //make a new one
                    theSettings = new UserSettings
                    {
                        Locale = myLocales.CurrentLocale.Locale,
                        Skin = "SWG_DataPad",
                        Pallette = "Alpha Blue",
                        GhostFont = "Aurek-Besh"
                    };
                  
                    //and save that into the file                        
                    using (MemoryStream msOut = new MemoryStream())
                    using (IsolatedStorageFileStream outStream = new IsolatedStorageFileStream("settings.cfg", FileMode.OpenOrCreate, System.IO.FileAccess.Write, inFile))
                    using (BinaryWriter bwOut = new BinaryWriter(outStream))
                    {
                        //serialize it                        
                        bf.Serialize(msOut, theSettings);

                        //encrypt it and write it out
                        bwOut.Write(myEncryption.Encrypt(msOut.ToArray()));
                    }
                 
                }

                //and put it in the resources
                App.Current.Resources.Add("UserPreferences", theSettings);
            }
            else
            {
                //collect them from the Application Resources Collection
                theSettings = App.Current.Resources["UserPreferences"] as UserSettings;
            }
        }

        public void UpdateSettings(SettingsType theType, object theSetting)
        {
            //collect them from the Application Resources Collection
            theSettings = App.Current.Resources["UserPreferences"] as UserSettings;

            switch (theType)
            {
                case SettingsType.AddCustomServer:
                case SettingsType.RemoveCustomServer:
                    //don't all any duplicates
                    theSettings.CustomServers.RemoveAll(cs => cs.ServerId == ((CustomServer)theSetting).ServerId);

                    if (theType == SettingsType.AddCustomServer)
                    {
                        theSettings.CustomServers.Add(theSetting as CustomServer);
                    }                   
                    break;
                case SettingsType.AddServerCredentials:
                case SettingsType.RemoveServerCredentials:
                    //get a serverdetail object
                    ServerDetails addMe = theSetting as ServerDetails;

                    //don't allow duplicate username for this server
                    theSettings.Servers.RemoveAll(sd => sd.ServerId == addMe.ServerId && sd.Username.Equals(addMe.Username, StringComparison.InvariantCultureIgnoreCase));

                    if (theType == SettingsType.AddServerCredentials)
                    {
                        theSettings.Servers.Add(addMe);
                    }
                    break;
                default:
                    //update the setting
                    theSettings.GetType().GetProperty(theType.ToString()).SetValue(theSettings, theSetting, null);
                    break;
            }
        
            App.Current.Resources["UserPreferences"] = theSettings;

            BinaryFormatter bf = new BinaryFormatter();
            LauncherEncryption myEncryption = new LauncherEncryption();
            IsolatedStorageFile inFile = IsolatedStorageFile.GetUserStoreForAssembly();

            //and save that into the file                        
            using (MemoryStream msOut = new MemoryStream())
            using (IsolatedStorageFileStream outStream = new IsolatedStorageFileStream("settings.cfg", FileMode.OpenOrCreate, System.IO.FileAccess.Write, inFile))
            using (BinaryWriter bwOut = new BinaryWriter(outStream))
            {
                //serialize it                        
                bf.Serialize(msOut, theSettings);

                //encrypt it and write it out
                bwOut.Write(myEncryption.Encrypt(msOut.ToArray()));
            }
        }

        public string UserLocale
        {
            get
            {                
                return theSettings.Locale;
            }
        }

        public string CurrentSkin
        {
            get
            {                
                return theSettings.Skin;
            }
        }

        public Guid DefaultServer
        {
            get
            {
                return theSettings.DefaultServer;
            }
        }

        public UISettings.Pallette CurrentPallette
        {
            get
            {               
                return mySettings.GetPallette(theSettings.Pallette);
            }
        }

        public UISettings.StarWarsFont CurrentGhostFont
        {
            get
            {
                return mySettings.GetFont(theSettings.GhostFont);
            }
        }

        public List<ServerDetails> ServerCredentials
        {
            get
            {
                return theSettings.Servers;
            }
        }

        public List<ServerInfo> CachedServers
        {
            get
            {
                return theSettings.CachedServers;
            }
        }

        public List<StandardTre> CachedStandardTre
        {
            get
            {
                return theSettings.StandardTres;
            }
        }

        public string ClientPath
        {
            get
            {
                return theSettings.ClientPath;
            }
        }

        public List<ServerInfo> UserCustomServers
        {
            get
            {
                List<ServerInfo> lstCustomServers = new List<ServerInfo>();

                foreach (CustomServer theCustomServer in theSettings.CustomServers)
                {
                    lstCustomServers.Add(new ServerInfo
                    {
                        ServerName = theCustomServer.ServerName,
                        Address = theCustomServer.ServerAddress,
                        ServerId = theCustomServer.ServerId,
                        Port = theCustomServer.ServerPort,
                        Description = "Custom Server",
                        RSSFeedUrl = "http://www.swganh.com/anh_community/syndication.php?fid=12&t=1",
                        CharsCreated = 0,
                        LastUpdated = DateTime.Now,
                        LauncherPort = 0,
                        Population = 0,
                        SafeFolderName = theCustomServer.SafeFolderName
                    });
                }

                return lstCustomServers;
            }
        }

        public ServerInfo DefaultServerInformation
        {
            get
            {
                ServerInfo theServer = null;
                if (DefaultServer != Guid.Empty)
                {                     
                    theServer = theSettings.CachedServers.Find(ser => ser.ServerId == DefaultServer);
                }

                if (theServer == null)
                {
                    //try and get it from the custom server list
                    theServer = UserCustomServers.Find(ser => ser.ServerId == DefaultServer);
                }
                return theServer;
            }
        }
    }   

    [Serializable]
    public class UserSettings
    {
        public string Skin
        {
            get;
            set;
        }

        public string Pallette
        {
            get;
            set;
        }

        public string GhostFont
        {
            get;
            set;
        }

        public string Locale
        {
            get;
            set;
        }

        public string ClientPath
        {
            get;
            set;
        }

        private Guid guDefaultServer = Guid.Empty;

        public Guid DefaultServer
        {
            get
            {
                return guDefaultServer;
            }
            set
            {
                guDefaultServer = value;
            }
        }

        private List<CustomServer> lstCustomServers;

        public List<CustomServer> CustomServers
        {
            get
            {
                if (lstCustomServers == null)
                {
                    lstCustomServers = new List<CustomServer>();
                }
                return lstCustomServers;
            }
            set
            {
                lstCustomServers = value;
            }
        }

        public List<ServerInfo> CachedServers
        {
            get;
            set;
        }

        public List<StandardTre> StandardTres
        {
            get;
            set;
        }

        private List<ServerDetails> lstServers;

        public List<ServerDetails> Servers
        {
            get
            {
                if (lstServers == null)
                {
                    lstServers = new List<ServerDetails>();
                }
                return lstServers;
            }
            set
            {
                lstServers = value;
            }
        }

        public void AddServer(ServerDetails theServer)
        {
            if (!Servers.Any(serv => serv.ServerId == theServer.ServerId))
            {
                Servers.Add(theServer);
            }
        }
    }

    [Serializable]
    public class CustomServer
    {
        public string ServerName
        {
            get;
            set;
        }
        public string ServerAddress
        {
            get;
            set;
        }

        public int ServerPort
        {
            get;
            set;
        }

        public Guid ServerId
        {
            get;
            set;
        }

        public string SafeFolderName
        {
            get
            {
                string strSafeFolderName = "";
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
        }
    }


    [Serializable]
    public class ServerDetails
    {
        public Guid ServerId
        {
            get;
            set;
        }

        public string ServerName
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        private List<string> lstCharacters;

        public List<string> Characters
        {
            get
            {
                if (lstCharacters == null)
                {
                    lstCharacters = new List<string>();
                }
                return lstCharacters;
            }
            set
            {
                lstCharacters = value;
            }
        }

        public void AddCharacter(string strCharacterName)
        {
            Characters.Add(strCharacterName);
        }
    }    
}
