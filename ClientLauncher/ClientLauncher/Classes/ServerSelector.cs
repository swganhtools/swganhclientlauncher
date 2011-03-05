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
using ClientLauncher.Usercontrols;
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
    ///     <MyNamespace:ServerSelector/>
    ///
    /// </summary>
    public class ServerSelector : RadioButton
    {
        static ServerSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ServerSelector), new FrameworkPropertyMetadata(typeof(ServerSelector)));
        }

        public ServerSelector(ServerInfo theServerInfo, UserPreferences thePreferences, bool blnAllowCustomServerUpdate, TextVariables theTextVariables)
        {
            TheServerInfo = theServerInfo;
            TheUserPreferences = thePreferences;
            AllowCustomServerUpdate = blnAllowCustomServerUpdate;
            TheTextVariables = theTextVariables;
        }

        public TextVariables TheTextVariables
        {
            get;
            set;
        }

        public bool AllowCustomServerUpdate
        {
            get;
            set;
        }

        public UserPreferences TheUserPreferences
        {
            get;
            set;
        }

        public ServerInfo TheServerInfo
        {
            get;
            set;
        }        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Grid leGrid = (Grid)this.Template.FindName("grdPlanet", this);

            Random myRandom = new Random();

            Server.PlanetType thePlanet = Server.PlanetType.Tatooine;
            switch (myRandom.Next(0, 5))
            {
                case 1:
                    thePlanet = Server.PlanetType.Mars;
                    break;
                case 2:
                    thePlanet = Server.PlanetType.Uranus;
                    break;
                case 3:
                    thePlanet = Server.PlanetType.Venus;
                    break;
                default:
                    thePlanet = Server.PlanetType.Tatooine;
                    break;
            }           

            Server theServer = new Server(thePlanet, TheServerInfo);

            leGrid.Children.Add(theServer);
        }       
    }
}
