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

namespace ClientLauncher.Usercontrols
{
    /// <summary>
    /// Interaction logic for ClientPatcher.xaml
    /// </summary>
    public partial class ClientPatcher : UserControl
    {
        #region Private Variables

        private Thickness thck = new Thickness(0, 0, 0, 5);

        #endregion

        private UserPreferences myUserPrefs;

        private TextVariables myVariables
        {
            get
            {
                return this.DataContext as TextVariables;
            }
        }      

        public ClientPatcher(UserPreferences theUserPrefs)
        {
            InitializeComponent();

            myUserPrefs = theUserPrefs;
            this.Loaded += new RoutedEventHandler(ClientPatcher_Loaded);
        }

   
        private void AddServersFromList(List<ServerInfo> lstServers)
        {
            Binding myBinding = new Binding("RefreshServers");
            myBinding.Source = this.DataContext;
            btnRefreshList.SetBinding(Button.ContentProperty, myBinding);
            btnRefreshList.IsEnabled = true;
            foreach (ServerInfo theServer in lstServers)
            {
                if (App.Current.Resources["ServerType"].ToString().Equals("Textual", StringComparison.InvariantCultureIgnoreCase))
                {
                    AddTextualServer(theServer);
                }
                else
                {
                    AddServer(theServer);
                }
            }
        }

        void ClientPatcher_Loaded(object sender, RoutedEventArgs e)
        {
            wpServers.Children.Clear();
            spServers.Children.Clear();
            AddServersFromList(myUserPrefs.CachedServers);
            AddServersFromList(myUserPrefs.UserCustomServers);
        }

        private void GetServersCompleted(IAsyncResult theResult)
        {
            try
            {
                LauncherData.LauncherDataClient myClient = theResult.AsyncState as LauncherData.LauncherDataClient;
                List<LauncherData.ServerInfo> lstLiveServers = myClient.EndGetServers(theResult);

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

                List<ServerInfo> lstServers = tmpServers.ToList();

                if (lstServers.Count > 0)
                {
                    myUserPrefs.UpdateSettings(UserPreferences.SettingsType.CachedServers, lstServers);
                    this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                    {
                        wpServers.Children.Clear();
                        spServers.Children.Clear();
                        AddServersFromList(lstServers);

                    }), System.Windows.Threading.DispatcherPriority.Normal);

                }
            }
            catch (Exception exGeneral)
            {
                this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    ColorConverter cc = new ColorConverter();
                    TextBlock tbNoServers = new TextBlock();
                    tbNoServers.Foreground = (SolidColorBrush)App.Current.Resources["LightFill"];
                    tbNoServers.FontFamily = new FontFamily("Verdana");
                    tbNoServers.FontSize = 14;
                    tbNoServers.Text = myVariables.NotConnected + " @ " + exGeneral.Message;
                    tbNoServers.TextWrapping = TextWrapping.Wrap;
                    wpServers.Children.Add(tbNoServers);
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        private void btnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            Binding myBinding = new Binding("RefreshingServers");
            myBinding.Source = this.DataContext;
            btnRefreshList.SetBinding(Button.ContentProperty, myBinding);
            btnRefreshList.IsEnabled = false;
            ServiceMaker myServiceMaker = new ServiceMaker();
            LauncherData.LauncherDataClient myClient = myServiceMaker.GetServiceClient();
            myClient.BeginGetServers(new AsyncCallback(GetServersCompleted), myClient);
        }

        private void AddTextualServer(ServerInfo theServerInfo)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                svwpServers.Visibility = Visibility.Collapsed;
                svspServers.Visibility = Visibility.Visible;
                TextualServerControl theControl = new TextualServerControl(theServerInfo, myUserPrefs, false, myVariables);
                theControl.DataContext = theServerInfo;
                theControl.Checked += new RoutedEventHandler(theControl_Checked);
                theControl.Margin = thck;
                spServers.Children.Add(theControl);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }
        private void AddServer(ServerInfo theServerInfo)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                svspServers.Visibility = Visibility.Collapsed;
                svwpServers.Visibility = Visibility.Visible;
                ServerSelector theControl = new ServerSelector(theServerInfo, myUserPrefs, false, myVariables);
                theControl.Checked += new RoutedEventHandler(theControl_Checked);
                theControl.Margin = thck;
                wpServers.Children.Add(theControl);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        void theControl_Checked(object sender, RoutedEventArgs e)
        {
            ServerInfo theServerInfo = null;
            brdServerInfo.Visibility = Visibility.Visible;
            ServerSelector theServer = sender as ServerSelector;
            if (theServer == null)
            {
                RadioButton rbIWasSelected = sender as RadioButton;
                theServerInfo = rbIWasSelected.DataContext as ServerInfo;
            }
            else
            {
                theServerInfo = theServer.TheServerInfo;
            }         

            grdPatcher.Children.Clear();
            PatchProgress myPatchProgress = new PatchProgress(theServerInfo, myUserPrefs);
            myPatchProgress.DataContext = this.DataContext;
            myPatchProgress.Loaded += new RoutedEventHandler(myPatchProgress_Loaded);
            myPatchProgress.OnError += new EventHandler<ErrorMessageEventArgs>(myPatchProgress_OnError);
            myPatchProgress.PatchComplete += new EventHandler<Patcher.PatchFunctionCompleteEventArgs>(myPatchProgress_PatchComplete);
            grdPatcher.Children.Add(myPatchProgress);
            rctOverlay.Visibility = Visibility.Visible;
        }

        public event EventHandler PatchStarted;
        public event EventHandler PatchCompleted;

        public void PatchComplete(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                rctOverlay.Visibility = Visibility.Collapsed;
            }), System.Windows.Threading.DispatcherPriority.Normal);
            
        }

        void myPatchProgress_PatchComplete(object sender, Patcher.PatchFunctionCompleteEventArgs e)
        {
            if (OnError != null)
            {
                OnError(this, new ErrorMessageEventArgs(e.Message, new EventHandler(PatchComplete)));
            }
            if (PatchCompleted != null)
            {
                PatchCompleted(this, new EventArgs());
            }
        }

        void myPatchProgress_OnError(object sender, ErrorMessageEventArgs e)
        {
            if (OnError != null)
            {
                OnError(sender, e);
            }
        }

        public event EventHandler<ErrorMessageEventArgs> OnError;

        void myPatchProgress_Loaded(object sender, RoutedEventArgs e)
        {
            PatchProgress iWasLoaded = sender as PatchProgress;
            iWasLoaded.StartPatch();
            if (PatchStarted != null)
            {
                PatchStarted(this, new EventArgs());
            }
        }
       
    }
}
