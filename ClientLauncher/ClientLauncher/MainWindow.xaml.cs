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
using System.Windows.Shapes;
using ClientLauncher.Usercontrols;
using System.Windows.Media.Animation;
using System.IO;
using System.Diagnostics;

namespace ClientLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UISettings mySettings;
        private UserPreferences myUserPrefs;
        private List<ImageBrush> lstImageBrushes;
        private int nCurrentImage = 0;
        private Storyboard sbFadeOut;
        private Storyboard sbFadeIn;
        private Locales myLocales;
        private List<LauncherData.LauncherVersion> lstLatestVersions;
        #region UI Controls
        private ClientPatcher myClientPatcher;
        #endregion
        public MainWindow(List<LauncherData.LauncherVersion> theLastestVersions)
        {
            InitializeComponent();
            lblStatus.Text = "v " + ApplicationUpdates.CurrentVersion;

            lstLatestVersions = theLastestVersions;

            //sort out the prefs and langauges
            myUserPrefs = new UserPreferences();
            myLocales = new Locales(myUserPrefs.UserLocale);
            this.DataContext = myLocales.CurrentLocale;           

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(MainWindow_DataContextChanged);
                        
            //ok, find our storyboards
            sbFadeOut = this.Resources["sbFadeOut"] as Storyboard;
            sbFadeIn = this.Resources["sbFadeIn"] as Storyboard;
            
            //and make sure they notify us when they're finished animating
            sbFadeOut.Completed += new EventHandler(sbFadeOut_Completed);
            sbFadeIn.Completed += new EventHandler(sbFadeIn_Completed);

            //have this fire when everythings been rendered and is ready for action
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);            
        }

        void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //must have come from the languages bit
            txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).ChangeLanguage.ToUpper();
        }
             
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {           
            ResetSettings();

            mySettings.SetNewPallette(myUserPrefs.CurrentPallette);
            mySettings.SetGhostFont(myUserPrefs.CurrentGhostFont.Name);

            if (lstLatestVersions.Count > 0)
            {
                //combine the patch notes
                string strPatchNotes = "";
                foreach (LauncherData.LauncherVersion theVersion in lstLatestVersions)
                {
                    strPatchNotes += theVersion.VersionNumber + " (" + theVersion.DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + ")" + Environment.NewLine;
                    strPatchNotes += theVersion.PatchNotes + Environment.NewLine + Environment.NewLine;
                }

                this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    lblMessage.Text = myLocales.CurrentLocale.DownloadingUpdateOne + Environment.NewLine + myLocales.CurrentLocale.DownloadingUpdateTwo;
                    lblMessage.Text += Environment.NewLine + Environment.NewLine + strPatchNotes;                    
                    brdMessageBox.Visibility = Visibility.Visible;

                    messageboxOK.Visibility = Visibility.Collapsed;
                    ServiceMaker myServiceMaker = new ServiceMaker();
                    ApplicationUpdates myApplicationUpdates = new ApplicationUpdates(myServiceMaker.GetServiceClient());
                    myApplicationUpdates.OnDownloadProgress += new EventHandler<ApplicationUpdates.DownloadProgressEventArgs>(myApplicationUpdates_OnDownloadProgress);
                    myApplicationUpdates.OnDownloadComplete += new EventHandler<ApplicationUpdates.DownloadCompleteEventArgs>(myApplicationUpdates_OnDownloadComplete);
                    myApplicationUpdates.UpdateToLatestVersion(this.Dispatcher, lstLatestVersions.First());
                    prgDownload.Visibility = Visibility.Visible;
                    prgDownload.Maximum = lstLatestVersions.First().FileSize;
                    prgDownload.Value = 0;

                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
            else
            {
                if ((myUserPrefs.DefaultServer != Guid.Empty) && (myUserPrefs.DefaultServerInformation != null))
                {
                    //chek the game launcher, by default
                    scGameLauncher.IsChecked = true;
                }
                else
                {
                    scChooseGalaxy.IsChecked = true;
                }
            }
        }

        void myApplicationUpdates_OnDownloadComplete(object sender, ApplicationUpdates.DownloadCompleteEventArgs e)
        {
            Process.Start(e.ExecutablePath);

            App.Current.Shutdown();
        }    

        void myApplicationUpdates_OnDownloadProgress(object sender, ApplicationUpdates.DownloadProgressEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {               
                prgDownload.Value = e.Downloaded;                

            }), System.Windows.Threading.DispatcherPriority.Normal);            
        }

        private void ResetSettings()
        {
            //Stop the animation
            sbFadeIn.Stop();
            sbFadeOut.Stop();

            //rest for to original values
            ResetForm();

            //make the setting obejct
            mySettings = new UISettings(myUserPrefs.CurrentSkin);            

            List<string> lstPaths = mySettings.SetupMainForm(pthBorder, grdTitle, grdWindowControls, grdStatusBar, brdServerControls, null, formScale, brdMessageBox);

            if (lstPaths.Count > 0)
            {
                SetAnimation(lstPaths);
            }            
        }

        void scSettings_Checked(object sender, RoutedEventArgs e)
        {
            //show the preferences screen
            grdControls.Children.Clear();
     
            GamePreferences    myPrefs = new GamePreferences( mySettings, myUserPrefs);
            myPrefs.DataContext = this.DataContext;
            myPrefs.SettingsHaveChanged += new EventHandler<GamePreferences.SettingsChangedEventArgs>(myPrefs_SettingsHaveChanged);
            myPrefs.OnError += new EventHandler<ErrorMessageEventArgs>(myClientLauncher_OnError);
   
            grdControls.Children.Add(myPrefs);
            txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).Settings.ToUpper();
        }


        private void ResetForm()
        {
            pthBorder.Fill = null;
            pthBorder.StrokeThickness = 0;
            pthBorder.Data = App.Current.Resources["MainBorderPath"] as Geometry;
            grdTitle.Clip = null;
            grdWindowControls.Clip = null;
            brdMessageBox.Clip = null;
            brdServerControls.Margin = new Thickness(0);
            brdServerControls.BorderThickness = new Thickness(0);
            grdStatusBar.Clip = null;
            grdWindowControls.Margin = new Thickness(0);
            //formScale.ScaleX = 0.9375;
            //formScale.ScaleY = 0.8333;
            
        }

        void scLanguage_Checked(object sender, RoutedEventArgs e)
        {
            txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).ChangeLanguage.ToUpper();
            grdControls.Children.Clear();
            Languages myLanguages = new Languages(myUserPrefs);
            myLanguages.LocaleChanged += new EventHandler<Languages.LocaleChangedEventArgs>(myLanguages_LocaleChanged);
            
            grdControls.Children.Add(myLanguages);
        }

        void myLanguages_LocaleChanged(object sender, Languages.LocaleChangedEventArgs e)
        {
            this.DataContext = e.TheNewVariables;                      
        }

        void scPatcher_Checked(object sender, RoutedEventArgs e)
        {
            txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).ClientPatcher.ToUpper();
            grdControls.Children.Clear();
            myClientPatcher = new ClientPatcher(myUserPrefs);
            myClientPatcher.OnError += new EventHandler<ErrorMessageEventArgs>(myClientLauncher_OnError);
            grdControls.Children.Add(myClientPatcher);
            myClientPatcher.PatchStarted += new EventHandler(myClientPatcher_PatchStarted);
            myClientPatcher.PatchCompleted += new EventHandler(myClientPatcher_PatchCompleted);
        }

        void myClientPatcher_PatchCompleted(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                scPatcher.IsEnabled = scNews.IsEnabled = scLanguages.IsEnabled = scGameLauncher.IsEnabled = scChooseGalaxy.IsEnabled = scSettings.IsEnabled =  true;
            }), System.Windows.Threading.DispatcherPriority.Normal);             
        }

        void myClientPatcher_PatchStarted(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                scPatcher.IsEnabled = scNews.IsEnabled = scLanguages.IsEnabled = scGameLauncher.IsEnabled = scChooseGalaxy.IsEnabled = scSettings.IsEnabled = false;
            }), System.Windows.Threading.DispatcherPriority.Normal);     
        }

        void scNews_Checked(object sender, RoutedEventArgs e)
        {
            if ((myUserPrefs.DefaultServer != Guid.Empty) && (myUserPrefs.DefaultServerInformation != null))
            {
                txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).ServerNews.ToUpper();
                ServerNews myServerNews = new ServerNews();
                myServerNews.DataContext = this.DataContext;

                myServerNews.RefreshNews(myUserPrefs.DefaultServerInformation.RSSFeedUrl);
                grdControls.Children.Clear();
                grdControls.Children.Add(myServerNews);
            }
            else
            {
                myClientLauncher_OnError(sender, new ErrorMessageEventArgs(((TextVariables)this.DataContext).ChooseServerOne + Environment.NewLine + ((TextVariables)this.DataContext).ChooseServerTwo, new EventHandler(ShowServerList)));
                
                e.Handled = true;
            }
        }

        private void ShowServerList(object sender, EventArgs e)
        {
            scChooseGalaxy.IsChecked = true;
        }

        void scChooseGalaxy_Checked(object sender, RoutedEventArgs e)
        {
            //load the server list control
            grdControls.Children.Clear();

            ServerList myServerList = new ServerList(myUserPrefs);
            myServerList.DefaultServerSelected += new EventHandler(myServerList_DefaultServerSelected);
            grdControls.Children.Add(myServerList);
            txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).ChooseGalaxy.ToUpper();
        }

        void myServerList_DefaultServerSelected(object sender, EventArgs e)
        {
            //load the client launcher screen
            scGameLauncher.IsChecked = true;
        }

        private void SetAnimation(List<string> lstPaths)
        {
            //load all of the image brushes
            lstImageBrushes = new List<ImageBrush>();
            foreach (string strImagePath in lstPaths)
            {
                if (File.Exists(mySettings.RootPath + strImagePath))
                {
                    lstImageBrushes.Add(new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(mySettings.RootPath + strImagePath, UriKind.Relative))
                    });
                }
            }

            //ok, start our storyboards
            sbFadeOut.Begin();
        }

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            nCurrentImage++;
            if (nCurrentImage == lstImageBrushes.Count)
            {
                nCurrentImage = 0;
            }

            //loads the next image brush
            pthBorder.Fill = lstImageBrushes[nCurrentImage];
            
            sbFadeIn.Begin();
        }

        void sbFadeIn_Completed(object sender, EventArgs e)
        {
            sbFadeOut.Begin();
        }     

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();                
            }
        }

        private void btnToolBox_Click(object sender, RoutedEventArgs e)
        {
            //clear the server control's selectred item
            foreach (UIElement theElement in wrpServerControls.Children)
            {
                ServerControls theServerControl = theElement as ServerControls; ;
                if ((theServerControl != null) && (theServerControl.TheButtonType != ServerControls.ButtonType.Settings))
                {
                    theServerControl.IsChecked = false;
                }
                else
                {
                    theServerControl.IsChecked = true;
                }
            }
        }

        void myPrefs_SettingsHaveChanged(object sender, GamePreferences.SettingsChangedEventArgs e)
        {
            switch (e.WhatChanged)
            {
                case GamePreferences.ChangedSetting.Pallete:
                    mySettings.SetNewPallette(e.SettingName);
                    break;
                case GamePreferences.ChangedSetting.Skin:
                    ResetSettings();
                    break;
                case GamePreferences.ChangedSetting.GhostFont:
                    mySettings.SetGhostFont(e.SettingName);
                    break;
            }
        }

        private void btnMinimise_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClosewWindow_Click(object sender, RoutedEventArgs e)
        {            
            Application.Current.Shutdown();
        }

        private void scGameLauncher_Checked(object sender, RoutedEventArgs e)
        {
            if ((myUserPrefs.DefaultServer != Guid.Empty) && (myUserPrefs.DefaultServerInformation != null))
            {
                grdControls.Children.Clear();
                txtFunction.Text = txtGhostText.Text = ((TextVariables)this.DataContext).GameLauncher.ToUpper();
                Usercontrols.ClientLauncher myClientLauncher = new Usercontrols.ClientLauncher(myUserPrefs);
                myClientLauncher.OnError += new EventHandler<ErrorMessageEventArgs>(myClientLauncher_OnError);
                myClientLauncher.PatchStarted += new EventHandler(myClientPatcher_PatchStarted);
                myClientLauncher.PatchComplete += new EventHandler(myClientPatcher_PatchCompleted);
                grdControls.Children.Add(myClientLauncher);
            }
            else
            {
                myClientLauncher_OnError(sender, new ErrorMessageEventArgs(((TextVariables)this.DataContext).ChooseServerOne + Environment.NewLine + ((TextVariables)this.DataContext).ChooseServerTwo, new EventHandler(ShowServerList)));

                e.Handled = true;
            }
        }

     

        void myClientLauncher_OnError(object sender, ErrorMessageEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                lblMessage.Text = e.ErrorMessage;
                brdMessageBox.Visibility = Visibility.Visible;

                if (e.TheEvent != null)
                {
                    MessageBoxOk += e.TheEvent;
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);            
        }

        public event EventHandler MessageBoxOk;

        private void messageboxOK_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxOk != null)
            {
                MessageBoxOk(this, new EventArgs());                
            }
            MessageBoxOk = null;
            brdMessageBox.Visibility = Visibility.Collapsed;
        }
    }
}
