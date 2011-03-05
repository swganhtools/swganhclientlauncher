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

namespace ClientLauncher
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ClientLauncher"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ClientLauncher;assembly=ClientLauncher"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ServerControls/>
    ///
    /// </summary>
    public class ServerControls : RadioButton
    {

        private string strBindingElement = "";
        static ServerControls()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ServerControls), new FrameworkPropertyMetadata(typeof(ServerControls)));
        }

        public enum ButtonType
        {
            ChooseGalaxy,
            ClientPatcher,
            News,
            Language,
            Settings,
            ClientLauncher
        }

        public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(ServerControls));

        public string ToolTipText
        {
            get
            {
                return (string)GetValue(ToolTipTextProperty);
            }
            set
            {
                SetValue(ToolTipTextProperty, value);
            }
        }

        public ButtonType TheButtonType
        {
            get;
            set;
        }
        public ServerControls()
        {
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(ServerControls_DataContextChanged);
            this.GroupName = "ServerControls";
            this.Margin = new Thickness(4, 0, 4, 0);
        }

        void ServerControls_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToolTip myToolTip = new ToolTip();
            myToolTip.FontFamily = App.Current.Resources["FontFamily"] as FontFamily;
            myToolTip.FontSize = 16;
            myToolTip.FontWeight = FontWeights.Black;
            Binding myBinding = new Binding(strBindingElement);
            myBinding.Source = this.DataContext;
            myToolTip.SetBinding(System.Windows.Controls.ToolTip.ContentProperty, myBinding);
            this.ToolTip = myToolTip;
        }


        //public ServerControls(ButtonType theButtonType, string strToolTip)
        //{
        //    TheButtonType = theButtonType;
        //    this.GroupName = "ServerControls";
        //    ToolTipText = strToolTip;
        //}   
             
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //find the image
            Image leImage = (Image)this.Template.FindName("image", this);
            BitmapImage theSource = new BitmapImage();
            theSource.BeginInit();
            
            try
            {
                switch (TheButtonType)
                {
                    case ButtonType.ChooseGalaxy:
                        theSource.UriSource = new Uri("pack://application:,,,/Images/galaxy.png", UriKind.RelativeOrAbsolute);
                        strBindingElement = "ChooseGalaxy";
                        break;
                    case ButtonType.Language:
                        theSource.UriSource = new Uri("pack://application:,,,/Images/language.png", UriKind.RelativeOrAbsolute);
                        strBindingElement = "ChangeLanguage";
                        break;
                    case ButtonType.News:
                        theSource.UriSource = new Uri("pack://application:,,,/Images/info.png", UriKind.RelativeOrAbsolute);
                        strBindingElement = "ServerNews";
                        break;
                    case ButtonType.ClientPatcher:
                        theSource.UriSource = new Uri("pack://application:,,,/Images/check.png", UriKind.RelativeOrAbsolute);
                        strBindingElement = "ClientPatcher";
                        break;
                    case ButtonType.Settings:
                        theSource.UriSource = new Uri("pack://application:,,,/Images/settings.png", UriKind.RelativeOrAbsolute);
                        strBindingElement = "Settings";
                        break;
                    case ButtonType.ClientLauncher:
                        theSource.UriSource = new Uri("pack://application:,,,/Images/Launch.png", UriKind.RelativeOrAbsolute);
                        strBindingElement = "GameLauncher";
                        break;
                }
            }
            catch (Exception ex)
            {
                string atr = ex.Message;
            }
            theSource.EndInit();
            
            leImage.Source = theSource;    
        
            //set the tooltip if we have one
            if (!string.IsNullOrEmpty(strBindingElement))
            {
                ToolTip myToolTip = new ToolTip();
                myToolTip.FontFamily = App.Current.Resources["FontFamily"] as FontFamily;
                myToolTip.FontSize = 16;
                myToolTip.FontWeight = FontWeights.Black;
                Binding myBinding = new Binding(strBindingElement);
                myBinding.Source = this.DataContext;
                myToolTip.SetBinding(System.Windows.Controls.ToolTip.ContentProperty, myBinding);
                this.ToolTip = myToolTip;
            }
        }
 
    }
}
