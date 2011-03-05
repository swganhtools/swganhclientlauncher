using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Shapes;

namespace ClientLauncher
{
    public class ScreenResolution
    {

        #region Unmanaged functions and structs
        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(
                string deviceName, int modeNum, ref DEVMODE devMode);

        [StructLayout(LayoutKind.Sequential)]
        private struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        #endregion

        #region Managed Containers
        public class ScreenRes
        {
            public int Width
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }
            public int Colours
            {
                get;
                set;
            }
            public int Frequency
            {
                get;
                set;
            }

            public string DeviceName
            {
                get;
                set;
            }

            public string DisplayText
            {
                get
                {
                    return Width.ToString() + " x " + Height.ToString() + " @ " + Frequency.ToString() + "Hz";
                }

            }          
        }

        public class AvailableScreen
        {
            
            public string DisplayName
            {
                get;
                set;
            }

            public string RealDisplayName
            {
                get
                {
                    string strRealDisplay = "1";
                    if (!string.IsNullOrEmpty(DisplayName))
                    {
                        strRealDisplay = DisplayName.Replace("\\", "").Replace(".", "");
                    }

                    return strRealDisplay + (PrimaryDisplay ? " (Primary)" : "");
                }
            }

            public int AdapterNumber
            {
                get;
                set;
            }


            public bool PrimaryDisplay
            {
                get;
                set;
            }

            public List<ScreenRes> AvailableResolutions
            {
                get;
                set;
            }            
        }

        #endregion

        #region Helper Methods

        public List<AvailableScreen> GetScreenResolutions()
        {
            int nAdapter = 1;
            List<AvailableScreen> lstAvailbleScreens = new List<AvailableScreen>();

            foreach (System.Windows.Forms.Screen theScreen in System.Windows.Forms.Screen.AllScreens)
            {
                //get the device name

                string strDevice = theScreen.DeviceName;

                //and list all of the modes attached to it
                List<ScreenRes> lstScreens = new List<ScreenRes>();
                DEVMODE vDevMode = new DEVMODE();

                int i = 0;
                while (EnumDisplaySettings(strDevice, i, ref vDevMode))
                {
                    lstScreens.Add(new ScreenRes
                    {
                        Width = vDevMode.dmPelsWidth,
                        Height = vDevMode.dmPelsHeight,
                        Colours = 1 << vDevMode.dmBitsPerPel,
                        Frequency = vDevMode.dmDisplayFrequency,
                        DeviceName = vDevMode.dmDeviceName
                    });
                    i++;
                }

                //get a list of all the good resoultions
                // 1 : has the maximum amount of colours
                // 2 : fits into the main monitor size 
                // 3 : is a decent res >= 1024x768
                var goodRes = lstScreens.Where(scr => scr.Width >= 1024 && scr.Height >= 768 && scr.Colours == 65536 && scr.Width <= ((System.Drawing.Rectangle)theScreen.Bounds).Width && scr.Height <= ((System.Drawing.Rectangle)theScreen.Bounds).Height).Distinct(new ScreenResEquality());

                lstAvailbleScreens.Add(new AvailableScreen
                {
                    DisplayName = theScreen.DeviceName,
                    PrimaryDisplay = theScreen.Primary,
                    AdapterNumber = (theScreen.Primary ? 0 : nAdapter++),
                    AvailableResolutions = goodRes.ToList()
                });
            }

            return lstAvailbleScreens;
            
        }

        #endregion

    }

    class ScreenResEquality : IEqualityComparer<ScreenResolution.ScreenRes>
    {
        public bool Equals(ScreenResolution.ScreenRes x, ScreenResolution.ScreenRes y)
        {

            //if, exclusively, either are null then they can't be equal
            if ((Object.ReferenceEquals(x, null)) || (Object.ReferenceEquals(y, null)))
            {
                return false;
            }

            //if they're the same object then that's a match
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            //ok, test all the properties to see if they're a match
            return x.Width == y.Width && x.Height == y.Height && x.Frequency == y.Frequency && x.DeviceName.Equals(y.DeviceName) && x.Colours == y.Colours;
        }

        public int GetHashCode(ScreenResolution.ScreenRes theResolution)
        {
            if (Object.ReferenceEquals(theResolution, null))
            {
                return 0;
            }

            int nDeviceName = (String.IsNullOrEmpty(theResolution.DeviceName) ? 0 : theResolution.DeviceName.GetHashCode());

            return nDeviceName ^ theResolution.Colours.GetHashCode() ^ theResolution.Frequency.GetHashCode() ^ theResolution.Height.GetHashCode() ^ theResolution.Width.GetHashCode();
        }
    }
}
