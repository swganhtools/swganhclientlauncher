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
using System.Xml.Linq;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace ClientLauncher.Usercontrols
{
    /// <summary>
    /// Interaction logic for ServerNews.xaml
    /// </summary>
    public partial class ServerNews : UserControl
    {
        private TextVariables myVariables
        {
            get
            {
                return this.DataContext as TextVariables;
            }
        }
        public ServerNews()
        {
            InitializeComponent();

        }        

        private delegate List<RssItem> RefreshNewsDelegate(string strUrl);

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
                this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    txtStatus.Text = myVariables.RSSError + " " + ex.Message;
                    
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }           

            
            return lstRss;
        }

        public void RefreshNews(string strUrl)
        {
            txtStatus.Text = myVariables.LoadingWait;
            RefreshNewsDelegate del = new RefreshNewsDelegate(RefreshNewsAsych);

            AsyncCallback callback = new AsyncCallback(XMLHasFinished);
            del.BeginInvoke(strUrl, callback, null);


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


        private void XMLHasFinished(IAsyncResult theResult)
        {

            RefreshNewsDelegate del = (RefreshNewsDelegate)((AsyncResult)theResult).AsyncDelegate;
            List<RssItem> lstItems = del.EndInvoke(theResult);

            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                txtStatus.Text = myVariables.Loaded + " " + lstItems.Count.ToString(CultureInfo.InvariantCulture) + " " + myVariables.Items;
                lstNewsItems.ItemsSource = lstItems;

                //select the first item in the list box
                if (lstItems.Count > 0)
                {
                    lstNewsItems.SelectedIndex = 0;
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void lstNewsItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RssItem theItem = lstNewsItems.SelectedItem as RssItem;

            if (theItem != null)
            {
                blogText.ClearValue(FlowDocumentReader.DocumentProperty);
                try
                {
                    FlowDocument theDocument = XamlReader.Load(new XmlTextReader(new StringReader(theItem.Description))) as FlowDocument;
                    theDocument.Background = App.Current.Resources["LightAreaFill"] as RadialGradientBrush;

                    //insert the date and the title
                    Paragraph pgDateAndTitle = new Paragraph();
                    pgDateAndTitle.FontFamily = App.Current.Resources["FontFamily"] as FontFamily;
                    pgDateAndTitle.FontWeight = FontWeights.Bold;
                    pgDateAndTitle.FontSize = 20;
                    pgDateAndTitle.Inlines.Add(new Run(theItem.Title));
                    pgDateAndTitle.Inlines.Add(new LineBreak());
                    pgDateAndTitle.Inlines.Add(new Run(theItem.Published.ToString("yyyy-MM-dd HH:mm:ss")));
                    theDocument.Blocks.InsertBefore(theDocument.Blocks.FirstBlock, pgDateAndTitle);

                    ParseLinks(theDocument);

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

        void theLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink lnkIWasClicked = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(lnkIWasClicked.NavigateUri.AbsoluteUri));
            e.Handled = true;
            
        }

        private void btnFullArticle_Click(object sender, RoutedEventArgs e)
        {
             RssItem theItem = lstNewsItems.SelectedItem as RssItem;

             if (theItem != null)
             {
                 Process.Start(new ProcessStartInfo(theItem.Link));
                 e.Handled = true;
             }
        }
    }
}
