using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Resources;

namespace ClientLauncher
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        private Storyboard sbFirst;
        private Storyboard sbSecond;
        private Storyboard sbThird;
        private Storyboard sbFourth; 
        private Storyboard sbLanguagesLoading;
        private Storyboard sbSkinsLoading;
        private Storyboard sbLauncherUpdatesLoading;
        private Storyboard sbServersLoading;

        public class UIText
        {
            public string CheckingForUpdates
            {
                get;
                private set;
            }
            public string LoadingSkins
            {
                get;
                private set;
            }
            public string LoadingLanguages
            {
                get;
                private set;
            }
            public string LoadingServers
            {
                get;
                private set;
            }

            public string UpdateFound
            {
                get;
                private set;
            }
            public string Done
            {
                get;
                private set;
            }

            public UIText()
            {
                //load the user's locale
                UserPreferences myPreferences = new UserPreferences();
                string strLocale = myPreferences.UserLocale;

                if (string.IsNullOrEmpty(strLocale))
                {
                    //default the english
                    strLocale = "en-gb";
                }

                //load the XML
                StreamResourceInfo info = null;
                Uri myXML = new Uri("/lang_internal/" + strLocale + ".xml", UriKind.RelativeOrAbsolute);
                try
                {
                    info = Application.GetResourceStream(myXML);
                }
                catch
                {
                    //load english
                    info = Application.GetResourceStream(new Uri("/lang_internal/en-gb.xml", UriKind.RelativeOrAbsolute));
                }
                XmlReader read = XmlReader.Create(info.Stream);
                XElement theLang = XElement.Load(read);
                this.CheckingForUpdates = theLang.Element("CheckingForUpdates").Value + "...";
                this.Done = theLang.Element("Done").Value;
                this.LoadingLanguages = theLang.Element("LoadingLanguages").Value + "...";
                this.LoadingServers = theLang.Element("LoadingServers").Value + "...";
                this.LoadingSkins = theLang.Element("LoadingSkins").Value + "...";
                this.UpdateFound = theLang.Element("UpdateFound").Value;
                

            }
        }

        public Splash()
        {
            InitializeComponent();

            //load the strings for this locale
            this.DataContext = new UIText();

            //find the animations
            sbFirst = this.Resources["sbFirst"] as Storyboard;
            sbSecond = this.Resources["sbSecond"] as Storyboard;
            sbThird = this.Resources["sbThird"] as Storyboard;
            sbFourth = this.Resources["sbFourth"] as Storyboard;
            sbLanguagesLoading = this.Resources["sbLanguagesLoading"] as Storyboard;
            sbSkinsLoading = this.Resources["sbSkinsLoading"] as Storyboard;
            sbLauncherUpdatesLoading = this.Resources["sbLauncherUpdatesLoading"] as Storyboard;
            sbServersLoading = this.Resources["sbServersLoading"] as Storyboard;

            App.Current.SectionLoaded += new EventHandler<App.LoadingSectionCompleted>(Current_SectionLoaded);
            App.Current.SectionStarted += new EventHandler<App.LoadingSectionStarted>(Current_SectionStarted);
            this.Loaded += new RoutedEventHandler(Splash_Loaded);
        }

        void Current_SectionStarted(object sender, App.LoadingSectionStarted e)
        {
            Storyboard sbText = null;

            switch (e.Type)
            {
                case App.LoadingType.Skins:
                    sbText = sbSkinsLoading;
                    break;
                case App.LoadingType.Servers:
                    sbText = sbServersLoading;
                    break;
                case App.LoadingType.Launcher:
                    sbText = sbLauncherUpdatesLoading;
                    break;
                case App.LoadingType.Languages:
                    sbText = sbLanguagesLoading;
                    break;
            }

            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                sbText.Begin();
            }), System.Windows.Threading.DispatcherPriority.Normal);
            
        }

        void Current_SectionLoaded(object sender, App.LoadingSectionCompleted e)
        {

            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                switch (e.Type)
                {
                    case App.LoadingType.Launcher:
                        if (e.NumberLoaded == 0)
                        {
                            lblUpdates.Text += (this.DataContext as UIText).Done;
                        }
                        else
                        {
                            lblUpdates.Text += (this.DataContext as UIText).UpdateFound;
                        }                        
                        sbFirst.Begin();
                        break;
                    case App.LoadingType.Servers:
                        lblServers.Text += e.NumberLoaded.ToString();                        
                        sbFourth.Begin();                        
                        break;
                    case App.LoadingType.Skins:
                        lblSkins.Text += e.NumberLoaded.ToString();
                        sbSecond.Begin();                        
                        break;
                    case App.LoadingType.Languages:
                        lblLanguages.Text += e.NumberLoaded.ToString();
                        sbThird.Begin();                        
                        break;
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);
            
        }

        void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            IAsyncResult theResult = null;

            AsyncCallback initComplete = delegate(IAsyncResult aResult)
            {
                App.Current.ApplicationInitialise.EndInvoke(aResult);
                Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    Close();
                }), System.Windows.Threading.DispatcherPriority.Normal);
            };

            theResult = App.Current.ApplicationInitialise.BeginInvoke(this, initComplete, null);
        }

    }
}
