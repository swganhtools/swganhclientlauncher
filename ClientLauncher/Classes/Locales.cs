using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Reflection;
namespace ClientLauncher
{
    public class Locales
    {
        private string _CurrentLocale;
        private List<TextVariables> lstTextVariables;
        public Locales()
        {
            //set the english one as default
            _CurrentLocale = "en-gb";

            //load all the different locales
            LoadLocales();
        }

        public Locales(string strCurrentLocale)
        {
            //set the specified one as default
            _CurrentLocale = strCurrentLocale;

            //load all the different locales
            LoadLocales();
        }

        public int NumberLoaded
        {
            get;
            private set;
        }

        private void LoadLocales()
        {
            lstTextVariables = new List<TextVariables>();
            if (App.Current.Resources["TextVariables"] == null)
            {
                MemberInfo[] theMembers = new TextVariables().GetType().GetProperties();

                //how many locales do we have?
                string[] arFiles = Directory.GetFiles("lang", "*.xml");

                //load each one
                foreach (string strLocaleFile in arFiles)
                {
                    string strErrorMessage = "";

                    try
                    {
                        XElement theLanguage = XElement.Load(strLocaleFile);

                        TextVariables theVariables = new TextVariables();

                        foreach (MemberInfo theMemberInfo in theMembers)
                        {
                            try
                            {
                                //collect it from the data first
                                //most of them will be here
                                theVariables.GetType().GetProperty(theMemberInfo.Name).SetValue(theVariables, theLanguage.Element(theMemberInfo.Name).Value, null);
                            }
                            catch (Exception ex)
                            {
                                //some are also attributes though
                                try
                                {
                                    theVariables.GetType().GetProperty(theMemberInfo.Name).SetValue(theVariables, theLanguage.Attribute(theMemberInfo.Name).Value, null);
                                }
                                catch (Exception exGeneral)
                                {
                                    strErrorMessage += theMemberInfo.Name + "not found" + Environment.NewLine;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(strErrorMessage))
                        {
                            lstTextVariables.Add(theVariables);
                        }
                        else
                        {
                            MessageBox.Show("Error loading " + Path.GetFileName(strLocaleFile) + Environment.NewLine + strErrorMessage);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading " + strLocaleFile + "\r\n" + ex.Message);
                    }
                }

                if (App.Current.Resources["TextVariables"] == null)
                {
                    App.Current.Resources.Add("TextVariables", lstTextVariables);
                }
                else
                {
                    App.Current.Resources["TextVariables"] = lstTextVariables;
                }
            }
            else
            {
                lstTextVariables = App.Current.Resources["TextVariables"] as List<TextVariables>;
            }

            //and load the required one
            var currentLocales = lstTextVariables.Where(loc => loc.Locale.Equals(_CurrentLocale, StringComparison.InvariantCultureIgnoreCase));

            switch (currentLocales.Count())
            {
                case 0:
                    CurrentLocale = new TextVariables();
                    break;
                case 1:
                    CurrentLocale = currentLocales.First();
                    break;
                default:
                    MessageBox.Show("Multiple files found for " + _CurrentLocale);
                    CurrentLocale = currentLocales.First();
                    break;

            }

            //and record the amount that were loaded
            NumberLoaded = lstTextVariables.Count;
        }

        private void DefineLocales()
        {
            string strPrivateVariables = "private string _Name;\r\nprivate string _Locale;\r\n";
            string strPublicVariables = "public string Name\r\n\t{\r\n\tget\r\n\t{\r\n\tif(string.IsNullOrEmpty(_Name))\r\n\t{\r\n\t_Name = \"English\";\r\n\t}\r\n\treturn _Name;\r\n\t}set\r\n\t{\r\n\t_Name = value;\r\n\t}\r\n\t}public string Locale\r\n\t{\r\n\tget\r\n\t{\r\n\tif(string.IsNullOrEmpty(_Locale))\r\n\t{\r\n\t_Locale = \"en-gb\";\r\n\t}\r\n\treturn _Locale;\r\n\t}\r\n\tset\r\n\t{\r\n\t_Locale = value;\r\n\t}\r\n\t}";
            XElement theRoot = XElement.Load("lang/en-gb.xml");

            foreach (XElement theElement in theRoot.Elements())
            {
                strPrivateVariables += "private string _" + theElement.Name + ";\r\n";
                strPublicVariables += "public string " + theElement.Name + "\r\n{\r\n\tget\r\n\t{\r\n\tif(string.IsNullOrEmpty(_" + theElement.Name + "))\r\n\t{\r\n\t_" + theElement.Name + " = \"" + theElement.Value + "\";\r\n\t}\r\n\treturn _" + theElement.Name + ";\r\n\t}\r\n\tset\r\n\t{\r\n\t_" + theElement.Name + " = value;\r\n\t}\r\n\t}\r\n";
            }

            string strClassDefinition = "public class TextVariables\r\n{\r\n" + strPrivateVariables + "\r\n" + strPublicVariables + "}";

        }

        public TextVariables CurrentLocale
        {
            get;
            private set;
        }
    }
}
