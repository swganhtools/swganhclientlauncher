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

namespace ClientLauncher.Usercontrols
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList : UserControl
    {
        private Thickness thck = new Thickness(0, 0, 0, 5);
        private UserPreferences myUserPrefs;
        private ServerInfo currentServer;
        private TextVariables myTextVariables
        {
            get
            {
                return this.DataContext as TextVariables;
            }
        }
        public ServerList(UserPreferences theUserPrefs)
        {
            InitializeComponent();

            myUserPrefs = theUserPrefs;          
            
            this.Loaded += new RoutedEventHandler(ServerList_Loaded);
            
        }

        void ServerList_Loaded(object sender, RoutedEventArgs e)
        {
            wpServers.Children.Clear();
            spServers.Children.Clear();
            AddServersFromList(myUserPrefs.CachedServers);

            //and add any custom ones
            AddServersFromList(myUserPrefs.UserCustomServers);
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

                        //and add any custom ones
                        AddServersFromList(myUserPrefs.UserCustomServers);

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
                    tbNoServers.Text = myTextVariables.NotConnected + " @ " + exGeneral.Message;                    
                    tbNoServers.TextWrapping = TextWrapping.Wrap;
                    wpServers.Children.Add(tbNoServers);
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        private void AddTextualServer(ServerInfo theServerInfo)
        {            
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                svwpServers.Visibility = Visibility.Collapsed;
                svspServers.Visibility = Visibility.Visible;
                TextualServerControl theControl = new TextualServerControl(theServerInfo, myUserPrefs, true, myTextVariables);                
                theControl.DataContext = theServerInfo;
                theControl.Checked += new RoutedEventHandler(theControl_Checked);
                theControl.MouseDoubleClick += new MouseButtonEventHandler(theControl_MouseDoubleClick);
                theControl.Margin = thck;                
                spServers.Children.Add(theControl);
                if (myUserPrefs.DefaultServer == theServerInfo.ServerId)
                {
                    theControl.IsChecked = true;
                    theControl.BringIntoView();
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);
           
        }
        private void AddServer(ServerInfo theServerInfo)
        {            
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                svspServers.Visibility = Visibility.Collapsed;
                svwpServers.Visibility = Visibility.Visible;
                ServerSelector theControl = new ServerSelector(theServerInfo, myUserPrefs, true, myTextVariables);                
                theControl.Checked += new RoutedEventHandler(theControl_Checked);
                theControl.MouseDoubleClick += new MouseButtonEventHandler(theControl_MouseDoubleClick);                
                theControl.Margin = thck;               
                wpServers.Children.Add(theControl);
                if (myUserPrefs.DefaultServer == theServerInfo.ServerId)
                {
                    theControl.IsChecked = true;
                    theControl.BringIntoView();
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        void theControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ServerInfo theServerInfo = null;
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

            myUserPrefs.UpdateSettings(UserPreferences.SettingsType.DefaultServer, theServerInfo.ServerId);

            //lblServerName.Text = theServerInfo.ServerName + " (" + myTextVariables.DefaultServer + ")";

            if (DefaultServerSelected != null)
            {
                DefaultServerSelected(this, e);
            }
        }

        public event EventHandler DefaultServerSelected;

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

            //populate the labels
            lblServerName.Text = theServerInfo.ServerName;

            if (theServerInfo.ServerId == myUserPrefs.DefaultServer)
            {
                lblServerName.Text += " (" + myTextVariables.DefaultServer + ")"; ;
            }
            lblServerDescription.Text = theServerInfo.Description;
            lblServerBuild.Text = "1.0";
            lblServerPopulation.Text = theServerInfo.Population.ToString();

            currentServer = theServerInfo;

            if (myUserPrefs.UserCustomServers.Any(ucs => ucs.ServerId == theServerInfo.ServerId))
            {
                Storyboard sbShowCustomButtons = (Storyboard)this.Resources["ShowCustomServerButtons"];
                sbShowCustomButtons.Begin();
            }
            else
            {
                Storyboard sbHideButtons = (Storyboard)this.Resources["HideCustomServerButtons"];
                sbHideButtons.Begin();
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

        private void bntnAddCustomServer_Click(object sender, RoutedEventArgs e)
        {
            btnRefreshList.IsEnabled = false;
            brdCustomServer.Visibility = Visibility.Visible;
            svspServers.Visibility = Visibility.Collapsed;
            svwpServers.Visibility = Visibility.Collapsed;
            btnAdd.Tag = Guid.NewGuid().ToString();
            txtServerName.Focus();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool blnContinue = true;

            if (string.IsNullOrEmpty(txtServerPort.Text))
            {
                txtServerPort.Focus();
                blnContinue = false;
            }

            if (string.IsNullOrEmpty(txtServerAddress.Text))
            {
                txtServerAddress.Focus();
                blnContinue = false;
            }

            if (string.IsNullOrEmpty(txtServerName.Text))
            {
                txtServerName.Focus();
                blnContinue = false;
            }

            if (blnContinue)
            {
                btnRefreshList.IsEnabled = true;
                brdCustomServer.Visibility = Visibility.Collapsed;
                svspServers.Visibility = Visibility.Visible;
                svwpServers.Visibility = Visibility.Visible;

                //make a server info object
                CustomServer myCustomServer = new CustomServer();
                myCustomServer.ServerAddress = txtServerAddress.Text;
                myCustomServer.ServerId = new Guid(btnAdd.Tag.ToString());
                btnAdd.Tag = null;
                myCustomServer.ServerName = txtServerName.Text;
                int nServerPort = 0;
                int.TryParse(txtServerPort.Text, out nServerPort);
                myCustomServer.ServerPort = nServerPort;
                myUserPrefs.UpdateSettings(UserPreferences.SettingsType.AddCustomServer, myCustomServer);
                ServerList_Loaded(sender, e);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnRefreshList.IsEnabled = true;
            brdCustomServer.Visibility = Visibility.Collapsed;
            if (App.Current.Resources["ServerType"].ToString().Equals("Textual", StringComparison.InvariantCultureIgnoreCase))
            {
                svspServers.Visibility = Visibility.Visible;
            }
            else
            {
                svwpServers.Visibility = Visibility.Visible;
            }            
            
            btnAdd.Tag = null;

            if (myUserPrefs.UserCustomServers.Any(ucs => ucs.ServerId == currentServer.ServerId))
            {
                Storyboard sbShowCustomButtons = (Storyboard)this.Resources["ShowCustomServerButtons"];
                sbShowCustomButtons.Begin();
            }
        }

        private void btnEditServer_Click(object sender, RoutedEventArgs e)
        {
            //open up the custom server window
            txtServerAddress.Text = currentServer.Address;
            txtServerName.Text = currentServer.ServerName;
            txtServerPort.Text = currentServer.Port.ToString();

            btnRefreshList.IsEnabled = false;
            brdCustomServer.Visibility = Visibility.Visible;
            svspServers.Visibility = Visibility.Collapsed;
            svwpServers.Visibility = Visibility.Collapsed;
            btnAdd.Tag = currentServer.ServerId.ToString();
            txtServerName.Focus();
            Storyboard sbHideButtons = (Storyboard)this.Resources["HideCustomServerButtons"];
            sbHideButtons.Begin();
        }

        private void btnDeleteServer_Click(object sender, RoutedEventArgs e)
        {
            CustomServer myCustomServer = new CustomServer();
            myCustomServer.ServerId = currentServer.ServerId;
            //remove it from the list
            myUserPrefs.UpdateSettings(UserPreferences.SettingsType.RemoveCustomServer, myCustomServer);
            if (myUserPrefs.DefaultServer == currentServer.ServerId)
            {
                //clear that out too
                myUserPrefs.UpdateSettings(UserPreferences.SettingsType.DefaultServer, Guid.Empty);
            }
            currentServer = null;
            Storyboard sbHideButtons = (Storyboard)this.Resources["HideCustomServerButtons"];
            sbHideButtons.Begin();
            ServerList_Loaded(sender, new RoutedEventArgs());
        }
    }
}
