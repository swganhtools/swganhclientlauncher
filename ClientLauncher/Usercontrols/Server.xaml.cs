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
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class Server : UserControl
    {
        public Server(PlanetType thePlanetType, ServerInfo theServerInfo)
        {
            InitializeComponent();
            aPlanet = thePlanetType;
            this.Loaded += new RoutedEventHandler(Server_Loaded);
            lblServerName.Text = theServerInfo.ServerName;
        }

        void Server_Loaded(object sender, RoutedEventArgs e)
        {
            Ball ball = new Ball();
            ball.ImageSource = aPlanet.ToString();
            visualModel.Children.Add(ball);
        }

        private PlanetType aPlanet;

        public enum PlanetType
        {
            Mars,
            Tatooine,
            Uranus,
            Venus
        }
    }
}
