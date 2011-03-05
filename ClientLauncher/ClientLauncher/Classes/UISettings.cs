using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls;
namespace ClientLauncher
{
    /// <summary>
    /// Simple class to defin the settings from the skin XML file
    /// </summary>
    public class UISettings
    {
        private XElement theXmlSkin;
        private string strRootPath;
        private NumberFormatInfo ni;

        public string RootPath
        {
            get
            {
                return strRootPath;
            }
        }



        /// <summary>
        /// Describes the different types of background which can be used on the form
        /// </summary>
        public enum BackgroundType
        {
            /// <summary>
            /// Allows the form to have an image background
            /// </summary>
            Image,
            /// <summary>
            /// Allows the form to have a path-based background
            /// </summary>
            Path
        }

        #region Pallette
        [Serializable]
        public class Pallette
        {
            public string Name
            {
                get;
                set;
            }
            public string LightFill
            {
                get;
                set;
            }
            public string DarkText
            {
                get;
                set;
            }

            public string DarkFill
            {
                get;
                set;
            }

            public string MouseOver
            {
                get;
                set;
            }

            public string TitleArea
            {
                get;
                set;
            }

            public string ControlArea
            {
                get;
                set;
            }

            public string StatusArea
            {
                get;
                set;
            }

            public string TitleText
            {
                get;
                set;
            }

            public string[] DarkAreaFill
            {
                get;
                set;
            }

            public string[] LightAreaFill
            {
                get;
                set;
            }
        }
        #endregion        

        #region Star Wars Fonts

        [Serializable]
        public class StarWarsFont
        {
            public string Name
            {
                get;
                set;
            }

            public string FontPath
            {
                get;
                set;
            }
        }

        public StarWarsFont GetFont(string strFontName)
        {
            if (strFontName.Equals("Aurek-Besh", StringComparison.InvariantCultureIgnoreCase))
            {
                strFontName = "Aurabesh";
            }

            //dopped the trade federation one
            if (strFontName.Equals("Trade Federation"))
            {
                strFontName = "Sith Prophecy";
            }

            return GetFonts.Where(swf => swf.Name.Equals(strFontName, StringComparison.InvariantCultureIgnoreCase)).Single();
        }

        public List<StarWarsFont> GetFonts
        {
            get
            {
                if (App.Current.Resources["UIGhostFonts"] == null)
                {
                    List<StarWarsFont> lstSWFonts = new List<StarWarsFont>();
                    lstSWFonts.Add(new StarWarsFont
                    {
                        Name = "Aurabesh",
                        FontPath = "SWFonts/#Aurek-Besh"
                    });
                    lstSWFonts.Add(new StarWarsFont
                    {
                        Name = "Ewok",
                        FontPath = "./SWFonts/#Ewok"
                    });
                    lstSWFonts.Add(new StarWarsFont
                    {
                        Name = "Mandolarian",
                        FontPath = "./SWFonts/#Mandalorian"
                    });
                    lstSWFonts.Add(new StarWarsFont
                    {
                        Name = "Metal Rebel",
                        FontPath = "./SWFonts/#Metal Rebel"
                    });
                    lstSWFonts.Add(new StarWarsFont
                    {
                        Name = "New Futhork",
                        FontPath = "./SWFonts/#New Futhork"
                    });
                    lstSWFonts.Add(new StarWarsFont
                    {
                        Name = "Sith Prophecy",
                        FontPath = "./SWFonts/#Sith Prophecy"
                    });                 

                    App.Current.Resources.Add("UIGhostFonts", lstSWFonts);
                }

                return App.Current.Resources["UIGhostFonts"] as List<StarWarsFont>;
            }
        }

        #endregion

        public List<string> GetAvailableSkins
        {
            get
            {
                //get all the directories
                string[] arDirectories = Directory.GetDirectories("skins");

                //put them in a list
                List<string> lstDirectories = new List<string>();

                foreach (string strDirectory in arDirectories)
                {
                    lstDirectories.Add(strDirectory.Replace("skins\\",""));
                }

                return lstDirectories;
            }
        }

        public List<Pallette> GetAvailablePallettes
        {
            get
            {
                return App.Current.Resources["UIPalettes"] as List<Pallette>;
            }
        }

        public Pallette GetPallette(string strPalletteName)
        {
            Pallette defaultPallete = GetAvailablePallettes.Find(gap => gap.Name.Equals(strPalletteName, StringComparison.InvariantCultureIgnoreCase));

            if (defaultPallete == null)
            {
                defaultPallete = GetAvailablePallettes.First();
            }

            return defaultPallete;
        }

        public UISettings()
        {
            CultureInfo ci = new CultureInfo("en-GB");
            ni = (NumberFormatInfo)ci.NumberFormat.Clone();
            ni.NumberDecimalSeparator = ".";
        }

        public UISettings(string strSkinName)
        {
            //use the default skin if nothing passed
            if (String.IsNullOrEmpty(strSkinName))
            {
                strSkinName = "SWG_DataPad";
            }

            //set the root path
            strRootPath = "Skins" + Path.DirectorySeparatorChar + strSkinName + Path.DirectorySeparatorChar;

            if (File.Exists(strRootPath + "settings.xml"))
            {
                //and load the XML
                theXmlSkin = XElement.Load(strRootPath + "settings.xml");   
             
                //and load the Pallets
                XElement thePallettes = theXmlSkin.Element("Palettes");
                if (thePallettes == null)
                {
                    MessageBox.Show("No palettes defined\r\nUnable to continue");
                    Application.Current.Shutdown();
                }
                else
                {
                    try
                    {
                        List<Pallette> lstPallettes = new List<Pallette>();
                        foreach (XElement theElement in thePallettes.Elements())
                        {
                            lstPallettes.Add(new Pallette
                            {
                                Name = theElement.Attribute("Name").Value,
                                LightFill = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("LightFill")).Single().Attribute("Color").Value,
                                DarkFill = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("DarkFill")).Single().Attribute("Color").Value,
                                DarkText = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("DarkText")).Single().Attribute("Color").Value,
                                MouseOver = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("MouseOver")).Single().Attribute("Color").Value,
                                TitleArea = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("TitleArea")).Single().Attribute("Color").Value,
                                ControlArea = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("ControlArea")).Single().Attribute("Color").Value,
                                StatusArea = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("StatusArea")).Single().Attribute("Color").Value,
                                TitleText = theElement.Elements("SolidColorBrush").Where(ele => ele.Attribute("Key").Value.Equals("TitleText")).Single().Attribute("Color").Value,
                                DarkAreaFill = new string[] 
                            {
                                theElement.Elements("RadialGradientBrush").Where(ele => ele.Attribute("Key").Value.Equals("DarkAreaFill")).Single().Elements("GradientStop").Where(ele => ele.Attribute("Offset").Value.Equals("0")).Single().Attribute("Color").Value,
                                theElement.Elements("RadialGradientBrush").Where(ele => ele.Attribute("Key").Value.Equals("DarkAreaFill")).Single().Elements("GradientStop").Where(ele => ele.Attribute("Offset").Value.Equals("1")).Single().Attribute("Color").Value
                            },
                                LightAreaFill = new string[]
                            {
                                theElement.Elements("RadialGradientBrush").Where(ele => ele.Attribute("Key").Value.Equals("LightAreaFill")).Single().Elements("GradientStop").Where(ele => ele.Attribute("Offset").Value.Equals("0")).Single().Attribute("Color").Value,
                                theElement.Elements("RadialGradientBrush").Where(ele => ele.Attribute("Key").Value.Equals("LightAreaFill")).Single().Elements("GradientStop").Where(ele => ele.Attribute("Offset").Value.Equals("1")).Single().Attribute("Color").Value
                            }
                            });
                        }

                        if (App.Current.Resources["UIPalettes"] == null)
                        {
                            App.Current.Resources.Add("UIPalettes", lstPallettes);
                        }
                        else
                        {
                            App.Current.Resources["UIPalettes"] = lstPallettes;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }                
            }
        }
        public void SetNewPallette(string strPalletteName)
        {
            SetNewPallette(GetAvailablePallettes.Where(pal => pal.Name.Equals(strPalletteName, StringComparison.InvariantCultureIgnoreCase)).Single());
        }

        public void SetGhostFont(string strFontName)
        {
            App.Current.Resources["GhostFontFamily"] = new FontFamily(new Uri("pack://application:,,,/"), GetFont(strFontName).FontPath);
        }

        public void SetNewPallette(Pallette thePallette)
        {
            ColorConverter cv = new ColorConverter();
            App.Current.Resources["LightFill"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.LightFill));
            App.Current.Resources["DarkText"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.DarkText));
            App.Current.Resources["DarkFill"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.DarkFill));
            App.Current.Resources["TitleArea"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.TitleArea));
            App.Current.Resources["MouseOver"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.MouseOver));
            App.Current.Resources["ControlArea"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.ControlArea));
            App.Current.Resources["TitleText"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.TitleText));
            App.Current.Resources["StatusArea"] = new SolidColorBrush((Color)cv.ConvertFrom(thePallette.StatusArea));
            GradientStopCollection dark = new GradientStopCollection();
            dark.Add(new GradientStop
            {
                Color = (Color)cv.ConvertFrom(thePallette.DarkAreaFill[0]),
                Offset = 0
            });
            dark.Add(new GradientStop
            {
                Color = (Color)cv.ConvertFrom(thePallette.DarkAreaFill[1]),
                Offset = 1
            });
            GradientStopCollection light = new GradientStopCollection();
            light.Add(new GradientStop
            {
                Color = (Color)cv.ConvertFrom(thePallette.LightAreaFill[0]),
                Offset = 0
            });
            light.Add(new GradientStop
            {
                Color = (Color)cv.ConvertFrom(thePallette.LightAreaFill[1]),
                Offset = 1
            });

            App.Current.Resources["DarkAreaFill"] = new RadialGradientBrush(dark);
            App.Current.Resources["LightAreaFill"] = new RadialGradientBrush(light);
        }

        public List<string> SetupMainForm(System.Windows.Shapes.Path theBackgroundPath, Grid grdTitle, Grid grdWindowButtons, Grid grdStatusArea, Border brdServerControls, string strCurrentPallette, ScaleTransform formScale, Border brdMessageBox)
        {
            List<string> lstPaths = new List<string>();
            if (theXmlSkin != null)
            {
                //try and read the MainForm node
                XElement mainForm = theXmlSkin.Element("MainForm");

                if (mainForm == null)
                {
                    MessageBox.Show("Settings file is missing it's MainForm element\r\nUnable to continue");
                    Application.Current.Shutdown();
                }
                else
                {

                    //scale it
                    if (mainForm.Element("FormZoom") != null)
                    {
                        formScale.ScaleX = Convert.ToDouble(mainForm.Element("FormZoom").Element("x").Value, ni);
                        formScale.ScaleY = Convert.ToDouble(mainForm.Element("FormZoom").Element("y").Value, ni);
                    }

                    //set up the corner radiuseseses      
                    if (mainForm.Element("RoundedCornerRadius") != null)
                    {
                        double dblCornerBase = Convert.ToDouble(mainForm.Element("RoundedCornerRadius").Value, ni);
                        App.Current.Resources["BigCornerRadius"] = new CornerRadius(dblCornerBase);
                        App.Current.Resources["SmallCornerRadius"] = new CornerRadius(dblCornerBase / 2.0);
                        App.Current.Resources["TinyCornerRadius"] = new CornerRadius(dblCornerBase / 5.0);
                        App.Current.Resources["PrefsBorder"] = new CornerRadius(0, dblCornerBase, dblCornerBase, dblCornerBase);
                        App.Current.Resources["PrefsTabBorder"] = new CornerRadius(dblCornerBase / 5.0, dblCornerBase / 5.0, 0, 0);
                        App.Current.Resources["RectangleRadius"] = dblCornerBase;
                    }
                    else
                    {
                        //reset
                        CornerRadius cnrBase = new CornerRadius(0);
                        App.Current.Resources["BigCornerRadius"] = cnrBase;
                        App.Current.Resources["SmallCornerRadius"] = cnrBase;
                        App.Current.Resources["TinyCornerRadius"] = cnrBase;
                        App.Current.Resources["PrefsBorder"] = cnrBase;
                        App.Current.Resources["PrefsTabBorder"] = cnrBase;
                        App.Current.Resources["RectangleRadius"] = 0;
                    }

                    //make a ColorConverter to manage the string-to-color handling
                    ColorConverter cv = new ColorConverter();
            
                    //What's the default palette?
                    string strDefaultPalette = "Standard";

                    
                    XElement defaultPalette = mainForm.Element("DefaultPalette");
                    if (defaultPalette != null)
                    {
                        strDefaultPalette = defaultPalette.Value;
                    }

                    //do we have one from the cache maybe?
                    if (!String.IsNullOrEmpty(strCurrentPallette))
                    {
                        strDefaultPalette = strCurrentPallette;
                    }

                    //what type of server selector?
                    if (App.Current.Resources["ServerType"] == null)
                    {
                        App.Current.Resources.Add("ServerType", "Textual");
                    }
                    else
                    {
                        App.Current.Resources["ServerType"] = "Textual";
                    }
                    if (mainForm.Element("ServerType") != null)
                    {
                        App.Current.Resources["ServerType"] = mainForm.Element("ServerType").Value;
                    }

                    //Load it from the list
                    List<Pallette> lstPalletes = App.Current.Resources["UIPalettes"] as List<Pallette>;
                    var somePalettes = lstPalletes.Where(pal => pal.Name.Equals(strDefaultPalette, StringComparison.InvariantCultureIgnoreCase));
                    if (somePalettes.Count() > 0)
                    {
                        Pallette theDefaultPalette = somePalettes.First();
                        SetNewPallette(theDefaultPalette);
                    }

                    //and work out what type of MainForm is required
                    XElement formType = mainForm.Element("BackgroundType");

                    if (formType == null)
                    {
                        MessageBox.Show("Can't determine the BackgroundType\r\nUnable to continue");
                    }
                    else
                    {
                        switch (formType.Value.ToLowerInvariant())
                        {
                            case "image":

                                //here's a default one in case it all goes wrong
                                BitmapImage bmImage = new BitmapImage();
                                bmImage.BeginInit();
                                bmImage.UriSource = new Uri("pack://application:,,,/Images/background.jpg", UriKind.RelativeOrAbsolute);
                                bmImage.EndInit();
                                //do we have more than one image?
                                if (mainForm.Elements("BackgroundImage").Count() > 1)
                                {
                                    var imagePaths = from bi in mainForm.Elements("BackgroundImage")
                                                     select bi.Value;

                                    List<string> lstTempPaths = new List<string>();

                                    lstTempPaths = imagePaths.ToList();

                                    //do they all exists?
                                    foreach (string strImage in lstTempPaths)
                                    {
                                        if (File.Exists(strRootPath + strImage))
                                        {
                                            lstPaths.Add(strImage);
                                        }
                                    }

                                    //Load the first image
                                    ImageBrush bgImage = new ImageBrush();
                                    if (lstPaths.Count > 0)
                                    {                                        
                                        bgImage.ImageSource = new BitmapImage(new Uri(strRootPath + lstPaths[0], UriKind.Relative));                                        
                                    }
                                    else
                                    {
                                        bgImage.ImageSource = bmImage;
                                    }

                                    theBackgroundPath.Fill = bgImage;
                                }
                                else
                                {
                                    //Load the specified image
                                    ImageBrush bgImage = new ImageBrush();
                                    if (File.Exists(strRootPath + mainForm.Element("BackgroundImage").Value))
                                    {                                        
                                        bgImage.ImageSource = new BitmapImage(new Uri(strRootPath + mainForm.Element("BackgroundImage").Value, UriKind.Relative));
                                        
                                    }
                                    else
                                    {                                       
                                        bgImage.ImageSource = bmImage;
                                    }

                                    theBackgroundPath.Fill = bgImage;
                                }
                                break;
                            case "path":                                
                                //load the path geometry
                                theBackgroundPath.Data = Geometry.Parse(mainForm.Element("PathData").Value);
                                brdMessageBox.Clip = Geometry.Parse(mainForm.Element("MsgBoxPathData").Value);
                                if (mainForm.Element("BorderThickness") != null)
                                {
                                    theBackgroundPath.StrokeThickness = Convert.ToDouble(mainForm.Element("BorderThickness").Value, ni);                                    
                                }
                                if (mainForm.Element("BackgroundColor") != null)
                                {
                                    theBackgroundPath.Fill = new SolidColorBrush((Color)cv.ConvertFrom(mainForm.Element("BackgroundColor").Value));
                                }

                                if (mainForm.Element("TitlePath") != null)
                                {
                                    grdTitle.Clip = Geometry.Parse(mainForm.Element("TitlePath").Value);
                                }

                                if (mainForm.Element("WindowControlsPath") != null)
                                {
                                    grdWindowButtons.Clip = Geometry.Parse(mainForm.Element("WindowControlsPath").Value);
                                    grdWindowButtons.Margin = new Thickness(grdWindowButtons.Margin.Left, grdTitle.ActualHeight + theBackgroundPath.StrokeThickness, grdWindowButtons.Margin.Right, grdWindowButtons.Margin.Bottom);
                                }

                                if (mainForm.Element("StatusPath") != null)
                                {
                                    grdStatusArea.Clip = Geometry.Parse(mainForm.Element("StatusPath").Value);
                                }

                                if (mainForm.Element("StatusBorderThickness") != null)
                                {
                                    //make a new thickness
                                    Thickness thck = new Thickness();

                                    if (mainForm.Element("StatusBorderThickness").Element("Top") != null)
                                    {
                                        thck.Top = Convert.ToInt32(mainForm.Element("StatusBorderThickness").Element("Top").Value, ni);
                                    }

                                    if (mainForm.Element("StatusBorderThickness").Element("Bottom") != null)
                                    {
                                        thck.Bottom = Convert.ToInt32(mainForm.Element("StatusBorderThickness").Element("Bottom").Value, ni);
                                    }

                                    if (mainForm.Element("StatusBorderThickness").Element("Right") != null)
                                    {
                                        thck.Right = Convert.ToInt32(mainForm.Element("StatusBorderThickness").Element("Right").Value, ni);
                                    }

                                    if (mainForm.Element("StatusBorderThickness").Element("Left") != null)
                                    {
                                        thck.Left = Convert.ToInt32(mainForm.Element("StatusBorderThickness").Element("Left").Value, ni);
                                    }

                                    brdServerControls.BorderThickness = thck;
                                }
                                break;
                        }
                    } 
                }
            }

            return lstPaths;
            
        }        
    }
}
