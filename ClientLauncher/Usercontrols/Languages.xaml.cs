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
    /// Interaction logic for Languages.xaml
    /// </summary>
    public partial class Languages : UserControl
    {
        private List<TextVariables> lstVariables;

        private UserPreferences myPrefs;

        public Languages(UserPreferences parentPrefs)
        {
            InitializeComponent();
            lstVariables = App.Current.Resources["TextVariables"] as List<TextVariables>;
            this.Loaded += new RoutedEventHandler(Languages_Loaded);

            myPrefs = parentPrefs;
        }

        void Languages_Loaded(object sender, RoutedEventArgs e)
        {
            //pull all the languages into their respective radio button
            foreach (TextVariables theVariables in lstVariables)
            {
                Locale theLocale = new Locale(theVariables);
                if (myPrefs.UserLocale.Equals(theVariables.Locale, StringComparison.InvariantCultureIgnoreCase))
                {
                    theLocale.IsChecked = true;
                }
                theLocale.Checked += new RoutedEventHandler(theLocale_Checked);
                wpLanguages.Children.Add(theLocale);

               
            }

        }

        public class LocaleChangedEventArgs : EventArgs
        {
            public TextVariables TheNewVariables
            {
                get;
                private set;
            }

            public LocaleChangedEventArgs(TextVariables theVariables)
            {
                this.TheNewVariables = theVariables;
            }
        }

        public event EventHandler<LocaleChangedEventArgs> LocaleChanged;

        void theLocale_Checked(object sender, RoutedEventArgs e)
        {
            Locale iWasClicked = sender as Locale;

            if (iWasClicked != null)
            {
                myPrefs.UpdateSettings(UserPreferences.SettingsType.Locale, iWasClicked.LocaleVariables.Locale);

                if (LocaleChanged != null)
                {
                    LocaleChanged(this, new LocaleChangedEventArgs(iWasClicked.LocaleVariables));
                }
            }
        }      
    }
}
