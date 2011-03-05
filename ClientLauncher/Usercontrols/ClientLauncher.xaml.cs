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
using System.Diagnostics;
using System.Xml.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Net;

namespace ClientLauncher.Usercontrols
{
    /// <summary>
    /// Interaction logic for ClientLauncher.xaml
    /// </summary>
    public partial class ClientLauncher : UserControl
    {
        private UserPreferences theUserPreferences;

        public event EventHandler PatchStarted;
        public event EventHandler PatchComplete;

        public ClientLauncher(UserPreferences myUserPreferences)
        {
            InitializeComponent();

            _ServerInfo = myUserPreferences.DefaultServerInformation;
            theUserPreferences = myUserPreferences;

            //does this exe even exist yet?
            if (!File.Exists(SWGANHPAth + _ServerInfo.SafeFolderName + "\\swganh.exe"))
            {
                PatchProgress myPatchProgress = new PatchProgress(_ServerInfo, myUserPreferences);
                myPatchProgress.Loaded += new RoutedEventHandler(myPatchProgress_Loaded);
                myPatchProgress.OnError += new EventHandler<ErrorMessageEventArgs>(myPatchProgress_OnError);
                myPatchProgress.PatchComplete += new EventHandler<Patcher.PatchFunctionCompleteEventArgs>(myPatchProgress_PatchComplete);
                grdRightPanel.Children.Clear();
                grdRightPanel.Children.Add(myPatchProgress);
                btnLaunchClient.IsEnabled = false;

            }
            else
            {
                //read the first article
                RefreshNewsDelegate del = new RefreshNewsDelegate(RefreshNewsAsych);

                AsyncCallback callback = new AsyncCallback(XMLHasFinished);
                del.BeginInvoke(_ServerInfo.RSSFeedUrl, callback, null);
            }

            lblServerName.Text = _ServerInfo.ServerName;
            lblServerBuild.Text = "999";
            lblServerPopulation.Text = _ServerInfo.Population.ToString();

            //do we have any credentials for this server?
            if (myUserPreferences.ServerCredentials.Any(sc => sc.ServerId == _ServerInfo.ServerId))
            {
                cboUsername.ItemsSource = myUserPreferences.ServerCredentials.Where(sc => sc.ServerId == _ServerInfo.ServerId);
                cboUsername.SelectedIndex = 0;
            }
        }

        void myPatchProgress_PatchComplete(object sender, Patcher.PatchFunctionCompleteEventArgs e)
        {
            if (e.Success)
            {
                this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    btnLaunchClient.IsEnabled = true;
                }), System.Windows.Threading.DispatcherPriority.Normal);                
            }

            if (OnError != null)
            {
                OnError(sender, new ErrorMessageEventArgs(e.Message, null));
            }

            if (PatchComplete != null)
            {
                PatchComplete(this, e);
            }
        }

        void myPatchProgress_OnError(object sender, ErrorMessageEventArgs e)
        {
            if (OnError != null)
            {
                OnError(sender, e);
            }
        }

        void myPatchProgress_Loaded(object sender, RoutedEventArgs e)
        {
            PatchProgress iWasLoaded = sender as PatchProgress;
            if (iWasLoaded != null)
            {
                iWasLoaded.StartPatch();
            }

            if (PatchStarted != null)
            {
                PatchStarted(this, e);
            }
        }

        public void MessageBoxOK(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(strSWGANHPath))
            {
                System.Windows.Forms.FolderBrowserDialog dia = new System.Windows.Forms.FolderBrowserDialog();

                if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strSWGANHPath = dia.SelectedPath;
                }

                //add a slash if it needs one
                if (!strSWGANHPath.EndsWith("\\"))
                {
                    strSWGANHPath += "\\";
                }
            }
        }

        private List<RssItem> RefreshNewsAsych(string strUrl)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            WebResponse resp = req.GetResponse();
            Stream strIn = resp.GetResponseStream();
            StreamReader read = new StreamReader(strIn);
            string re = read.ReadToEnd();

            List<RssItem> lstRss = new List<RssItem>();
            //load the feed
            try
            {
                XElement theRoot = XElement.Parse(re);

                if (theRoot != null)
                {

                    //get the channel
                    XElement theChannel = theRoot.Element("channel");

                    if (theChannel != null)
                    {
                        //get the item elements
                        var theItems = from ele in theChannel.Elements("item")
                                       select new RssItem
                                       {
                                           Title = ele.Element("title").Value,
                                           Description = HtmlToXamlConverter.ConvertHtmlToXaml(ele.Element("description").Value, true),
                                           Link = ele.Element("link").Value,
                                           Published = DateTime.Parse(ele.Element("pubDate").Value)
                                       };

                        lstRss = theItems.ToList();
                    }
                }

            }
            catch (Exception ex)
            {
               
            }


            return lstRss;
        }

        private delegate List<RssItem> RefreshNewsDelegate(string strUrl);
        public void RefreshNews(string strUrl)
        {
            
            RefreshNewsDelegate del = new RefreshNewsDelegate(RefreshNewsAsych);

            AsyncCallback callback = new AsyncCallback(XMLHasFinished);
            del.BeginInvoke(strUrl, callback, null);


        }

        private void XMLHasFinished(IAsyncResult theResult)
        {
            RefreshNewsDelegate del = (RefreshNewsDelegate)((AsyncResult)theResult).AsyncDelegate;
            List<RssItem> lstItems = del.EndInvoke(theResult);

            if (lstItems.Count > 0)
            {
                theItem = lstItems.First();
            }

            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                if (theItem != null)
                {
                    blogText.ClearValue(FlowDocumentReader.DocumentProperty);
                    try
                    {
                        FlowDocument theDocument = XamlReader.Load(new XmlTextReader(new StringReader(theItem.Description))) as FlowDocument;
                        theDocument.Background = App.Current.Resources["LightAreaFill"] as RadialGradientBrush;
                        ParseLinks(theDocument);

                        //insert the date and the title
                        Paragraph pgDateAndTitle = new Paragraph();
                        pgDateAndTitle.FontFamily = App.Current.Resources["FontFamily"] as FontFamily;
                        pgDateAndTitle.FontWeight = FontWeights.Bold;
                        pgDateAndTitle.FontSize = 20;
                        pgDateAndTitle.Inlines.Add(new Run(theItem.Title));
                        pgDateAndTitle.Inlines.Add(new LineBreak());
                        pgDateAndTitle.Inlines.Add(new Run(theItem.Published.ToString("yyyy-MM-dd HH:mm:ss")));
                        theDocument.Blocks.InsertBefore(theDocument.Blocks.FirstBlock, pgDateAndTitle);

                        blogText.Document = theDocument;

                        btnFullArticle.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        Paragraph myErrorParagraph = new Paragraph();
                        myErrorParagraph.FontSize = 18;
                        myErrorParagraph.FontFamily = App.Current.Resources["FontFamily"] as FontFamily;
                        myErrorParagraph.Inlines.Add(new Run(myVariables.UnableToDisplayRSS));
                        myErrorParagraph.Inlines.Add(new LineBreak());
                        myErrorParagraph.Inlines.Add(new Run(ex.Message));
                        FlowDocument myErrorDocument = new FlowDocument();
                        myErrorDocument.Background = App.Current.Resources["LightAreaFill"] as RadialGradientBrush;
                        myErrorDocument.Blocks.Add(myErrorParagraph);
                        blogText.Document = myErrorDocument;                       
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        void theLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink lnkIWasClicked = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(lnkIWasClicked.NavigateUri.AbsoluteUri));
            e.Handled = true;

        }
        private void ParseLinks(FlowDocument theDocument)
        {
            foreach (Block theBlock in theDocument.Blocks)
            {
                Paragraph theParagraph = theBlock as Paragraph;
                if (theParagraph != null)
                {
                    //set the font family
                    theBlock.FontFamily = App.Current.Resources["FontFamily"] as FontFamily;
                    foreach (Inline theInline in theParagraph.Inlines)
                    {
                        Hyperlink theLink = theInline as Hyperlink;
                        if (theLink != null)
                        {
                            theLink.RequestNavigate += new RequestNavigateEventHandler(theLink_RequestNavigate);
                        }
                        else
                        {
                            //might be a span, and that might have further inlines
                            Span theSpan = theInline as Span;
                            if (theSpan != null)
                            {
                                foreach (Inline spaninlines in theSpan.Inlines)
                                {
                                    theLink = spaninlines as Hyperlink;
                                    if (theLink != null)
                                    {
                                        theLink.RequestNavigate += new RequestNavigateEventHandler(theLink_RequestNavigate);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        public class RssItem
        {
            public string Title
            {
                get;
                set;
            }
            public string Link
            {
                get;
                set;
            }
            public DateTime Published
            {
                get;
                set;
            }
            public string Description
            {
                get;
                set;
            }
        }

        private ServerInfo _ServerInfo;
        RssItem theItem;

        private void btnFullArticle_Click(object sender, RoutedEventArgs e)
        {
            if (theItem != null)
            {
                Process.Start(new ProcessStartInfo(theItem.Link));
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LaunchClient();
        }

        private void LaunchClient()
        {
            //change the server address
            IniFiles myFiles = new IniFiles(SWGANHPAth + _ServerInfo.SafeFolderName + "\\swg2uu_login.cfg");
            myFiles.WriteValue("ClientGame", "loginServerPort0", _ServerInfo.Port);
            myFiles.WriteValue("ClientGame", "loginServerAddress0", _ServerInfo.Address);

            if (chkAutoLogin.IsChecked.Value)
            {
                if ((string.IsNullOrEmpty(cboUsername.Text)) || (string.IsNullOrEmpty(txtPassword.Password)))
                {
                    string strErrorMessage = "";
                    if (string.IsNullOrEmpty(cboUsername.Text))
                    {
                        strErrorMessage += myVariables.BlankUsername;
                    }

                    if (string.IsNullOrEmpty(txtPassword.Password))
                    {
                        if (!string.IsNullOrEmpty(strErrorMessage))
                        {
                            strErrorMessage += Environment.NewLine;
                        }
                        strErrorMessage += myVariables.BlankPassword;

                    }

                    if (OnError != null)
                    {
                        OnError(this, new ErrorMessageEventArgs(strErrorMessage, null));
                    }
                }
                else
                {

                    btnLaunchClient.IsEnabled = false;

                    ServerConnector myConnector = new ServerConnector(this.DataContext as TextVariables);
                    myConnector.PacketReceived += new EventHandler<ServerConnector.PacketsReceivedEventArgs>(myConnector_PacketReceived);
                    if (!myConnector.CreateSessionKey(_ServerInfo, cboUsername.Text, txtPassword.Password))
                    {
                        if (OnError != null)
                        {
                            OnError(this, new ErrorMessageEventArgs(myConnector.Message, null));
                        }

                        btnLaunchClient.IsEnabled = true;

                    }
                }
            }
            else
            {
                //just start the client
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = SWGANHPAth + _ServerInfo.SafeFolderName + "\\swganh.exe";
                myProcess.StartInfo.WorkingDirectory = SWGANHPAth + _ServerInfo.SafeFolderName;
                //myProcess.StartInfo.Arguments = "-- -s Station sessionId=" + e.SessionKey;
                myProcess.Start();
            }
        }

        void myConnector_PacketReceived(object sender, ServerConnector.PacketsReceivedEventArgs e)
        {
            if (e.Success)
            {
                this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    //make the server details
                    ServerDetails myServerDetails = new ServerDetails();
                    myServerDetails.ServerId = _ServerInfo.ServerId;
                    myServerDetails.ServerName = _ServerInfo.ServerName;
                    myServerDetails.Username = cboUsername.Text;
                    myServerDetails.Password = txtPassword.Password;

                    if (chkRememberDetails.IsChecked.Value)
                    {
                        theUserPreferences.UpdateSettings(UserPreferences.SettingsType.AddServerCredentials, myServerDetails);
                    }
                    else
                    {
                        //maybe we need to removed them instead?
                        theUserPreferences.UpdateSettings(UserPreferences.SettingsType.RemoveServerCredentials, myServerDetails);
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);

                

                Process myProcess = new Process();
                myProcess.StartInfo.FileName = SWGANHPAth + _ServerInfo.SafeFolderName + "\\swganh.exe";
                myProcess.StartInfo.WorkingDirectory = SWGANHPAth + _ServerInfo.SafeFolderName;
                myProcess.StartInfo.Arguments = "-- -s Station sessionId=" + e.SessionKey;               
                myProcess.Start();                  
            }
            else
            {
                if (OnError != null)
                {
                    OnError(this, new ErrorMessageEventArgs(e.Message, null));
                }
                this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    btnLaunchClient.IsEnabled = true;
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        public event EventHandler<ErrorMessageEventArgs> OnError;

        private TextVariables myVariables
        {
            get
            {
                return this.DataContext as TextVariables;
            }
        }

        public delegate string GetClientDirectory(); 
        private string strSWGANHPath;
        private string SWGANHPAth
        {
            get
            {
                if (String.IsNullOrEmpty(strSWGANHPath))
                {
                    //try and find it in the registry
                    RegistryKey myRegistry = Registry.CurrentUser;
                    RegistryKey regSWGAppPath = myRegistry.OpenSubKey("SOFTWARE\\SWGANH Client");
                    if (regSWGAppPath == null)
                    {
                        //try and get it from their custom path
                        strSWGANHPath = theUserPreferences.ClientPath;

                        if (string.IsNullOrEmpty(strSWGANHPath))
                        {
                            //or ask the user to specify
                            if (OnError != null)
                            {
                                OnError(this, new ErrorMessageEventArgs(myVariables.RegistryErrorOne + Environment.NewLine + myVariables.RegistryErrorTwo + Environment.NewLine + "(e.g C:\\Program Files\\SWGANH Client", new EventHandler(MessageBoxOK)));
                            }
                        }
                        else
                        {
                            //add a slash if it needs one
                            if (!strSWGANHPath.EndsWith("\\"))
                            {
                                strSWGANHPath += "\\";
                            }
                        }
                    }
                    else
                    {
                        strSWGANHPath = regSWGAppPath.GetValue("").ToString();
                        //add a slash if it needs one
                        if (!strSWGANHPath.EndsWith("\\"))
                        {
                            strSWGANHPath += "\\";
                        }
                    }
                }

                return strSWGANHPath;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LaunchClient();
            }
        }

        private void chkAutoLogin_Checked(object sender, RoutedEventArgs e)
        {
            txtPassword.IsEnabled = true;
            cboUsername.IsEnabled = true;
        }

        private void chkAutoLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPassword.IsEnabled = false;
            cboUsername.IsEnabled = false;
        }

        private void cboUsername_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServerDetails myServerDetails = cboUsername.SelectedItem as ServerDetails;
            if (myServerDetails != null)
            {
                txtPassword.Password = myServerDetails.Password;
                chkRememberDetails.IsChecked = true;
            }
        }

        private void cboUsername_KeyDown(object sender, KeyEventArgs e)
        {
            //clear the password box
            txtPassword.Password = "";
        }      
    }
}
