using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace ClientLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ApplicationInitialise = InitiliseMyApp;            
        }

        public static new App Current
        {
            get
            {
                return Application.Current as App;
            }
        }

        internal delegate void ApplicationInitialiseDelegate(Splash theSplashScreen);
        internal ApplicationInitialiseDelegate ApplicationInitialise;

        public enum LoadingType
        {
            Launcher,
            Servers,
            Languages,
            Skins
        }

        public class LoadingSectionStarted : EventArgs
        {
            public LoadingType Type
            {
                get;
                private set;
            }

            public LoadingSectionStarted(LoadingType theType)
            {
                this.Type = theType;
            }

        }

        public class LoadingSectionCompleted : EventArgs
        {
            public LoadingType Type
            {
                get;
                private set;
            }

            public int NumberLoaded
            {
                get;
                private set;
            }

            public string Message
            {
                get;
                private set;
            }

            public LoadingSectionCompleted(LoadingType theType, int nLoaded, string strMessage)
            {
                this.Type = theType;
                this.NumberLoaded = nLoaded;
                this.Message = strMessage;
            }
        }

        public event EventHandler<LoadingSectionCompleted> SectionLoaded;
        public event EventHandler<LoadingSectionStarted> SectionStarted;


        private void InitiliseMyApp(Splash theSplashScreen)
        {
            //check for launcher updates first
            if (SectionStarted != null)
            {
                SectionStarted(this, new LoadingSectionStarted(LoadingType.Launcher));
            }
            UserPreferences myPrefs = new UserPreferences();
            ServiceMaker myServiceMaker = new ServiceMaker();
            LauncherData.LauncherDataClient myClient = myServiceMaker.GetServiceClient();

            ApplicationUpdates myApplicationUpdates = new ApplicationUpdates(myClient);

            List<LauncherData.LauncherVersion> lstLatestVersions = myApplicationUpdates.UpdateAvailable(this.Dispatcher);

            if (SectionLoaded != null)
            {
                SectionLoaded(this, new LoadingSectionCompleted(LoadingType.Launcher, lstLatestVersions.Count, ""));
            }

            //and skins
            if (SectionStarted != null)
            {
                SectionStarted(this, new LoadingSectionStarted(LoadingType.Skins));
            }
            UISettings mySettings = new UISettings();

            if (SectionLoaded != null)
            {
                SectionLoaded(this, new LoadingSectionCompleted(LoadingType.Skins, mySettings.GetAvailableSkins.Count, ""));
            }

            //and now the languages
            if (SectionStarted != null)
            {
                SectionStarted(this, new LoadingSectionStarted(LoadingType.Languages));
            }

            Locales myLocale = new Locales();

            if (SectionLoaded != null)
            {
                SectionLoaded(this, new LoadingSectionCompleted(LoadingType.Languages, myLocale.NumberLoaded, ""));
            }

            //finally, load the servers
            if (SectionStarted != null)
            {
                SectionStarted(this, new LoadingSectionStarted(LoadingType.Servers));
            }

            List<ServerInfo> lstServers = new List<ServerInfo>();


            try
            {
                List<LauncherData.ServerInfo> lstLiveServers = myClient.GetServers();

                
                

                var tmpServers = from ser in lstLiveServers
                                 select new ServerInfo
                                 {
                                     Address = ser.Address,
                                     CharsCreated = ser.CharsCreated,
                                     Description = ser.Description,
                                     LastUpdated = ser.LastUpdated,
                                     LauncherPort = ser.LauncherPort,
                                     Population = ser.Population,
                                     Port = ser.Port,
                                     RSSFeedUrl = ser.RSSFeedUrl,
                                     ServerId = ser.ServerId,
                                     ServerName = ser.ServerName
                                 };
                //update the preferecnes
                myPrefs.UpdateSettings(UserPreferences.SettingsType.CachedServers, tmpServers.ToList());
            }
            catch
            {
                //don't update the cached ones
            }            

            if (SectionLoaded != null)
            {
                SectionLoaded(this, new LoadingSectionCompleted(LoadingType.Servers, lstServers.Count, ""));
            }

            //and get the list of standard TRE files
            try
            {

                List<LauncherData.StandardTre> lstLiveStandardTres = myClient.GetStandardTre();
                var tmpStandard = from stre in lstLiveStandardTres
                                  select new StandardTre
                                  {
                                      Filename = stre.Filename,
                                      MD5Hash = stre.MD5Hash
                                  };

                myPrefs.UpdateSettings(UserPreferences.SettingsType.StandardTres, tmpStandard.ToList());
                
            }
            catch
            {

            }

            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                MainWindow = new MainWindow(lstLatestVersions);
                MainWindow.Show();
            }), System.Windows.Threading.DispatcherPriority.Normal);

        }
    }
}
