using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ClientLauncher
{
    public class Patcher : IDisposable
    {
        #region Private Variables
        private MD5CryptoServiceProvider hashmd5;
        private TextVariables myVariables;
        private bool isDisposed = false;
        private string strSWGANHPath = "";
        private ServerInfo cachedServerInfo;
        private UserPreferences myUserPreferences;
        #endregion

        #region Constructor / Deconstructor
        public Patcher(TextVariables theVariables, UserPreferences theUserPreferences)
        {
            myVariables = theVariables;
            hashmd5 = new MD5CryptoServiceProvider();
            myUserPreferences = theUserPreferences;
        }

        ~Patcher()
        {
            Dispose(false);
        }
        #endregion

        #region Dispose Methods
        public void Dispose()
        {
            OnError = null;
            PatchFunctionComplete = null;
            PatchStepFired = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }        

        private void Dispose(bool dispose)
        {
            if (!this.isDisposed)
            {
                if (dispose)
                {
                    //kill the MD5 object
                    hashmd5.Clear();
                }

                //remove the attached event
                PatchStepFired = null;

                this.isDisposed = true;
            }
        }
        #endregion

        #region Events and EventArgs
        public class PatchingEventArgs : EventArgs
        {
            public string Message
            {
                get;
                private set;
            }

            public bool NewEvent
            {
                get;
                private set;
            }

            public PatchingEventArgs(string strMessage, bool blnNewEvent)
            {
                this.Message = strMessage;
                this.NewEvent = blnNewEvent;
            }
        }

        public class PatchFunctionCompleteEventArgs : EventArgs
        {
            public string Message
            {
                get;
                private set;
            }

            public bool Success
            {
                get;
                private set;
            }

            public PatchFunctionCompleteEventArgs(string strMessage, bool blnSuccess)
            {
                this.Message = strMessage;
                this.Success = blnSuccess;
            }
        }

        public event EventHandler<PatchingEventArgs> PatchStepFired;
        public event EventHandler<PatchFunctionCompleteEventArgs> PatchFunctionComplete;
        public event EventHandler<ErrorMessageEventArgs> OnError;
        #endregion

        #region Public Properties

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

                //save this one
                myUserPreferences.UpdateSettings(UserPreferences.SettingsType.ClientPath, strSWGANHPath);
            }

            if (!string.IsNullOrEmpty(strSWGANHPath))
            {
                PatchClient(cachedServerInfo);
            }
        }

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
                        strSWGANHPath = myUserPreferences.ClientPath;

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

        #endregion

        #region Helper Classes
        public class CustomeTreCallObject
        {
            public LauncherData.LauncherDataClient TheClient
            {
                get;
                set;
            }

            public ServerInfo Server
            {
                get;
                set;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Asynnchronously checks the files and compares them against the set of standard MD5 checksums
        /// </summary>
        /// <param name="theServerInfo">ServerInfo onject of the server to patch for</param>
        public void PatchClient(ServerInfo theServerInfo)
        {
            //cache the ol' server info
            cachedServerInfo = theServerInfo;

            if (string.IsNullOrEmpty(SWGANHPAth))
            {                
                return;
            }
            //get a list of the custom TRE's
            ServiceMaker myServiceMaker = new ServiceMaker();
            LauncherData.LauncherDataClient myClient = myServiceMaker.GetServiceClient();
            CustomeTreCallObject theObject = new CustomeTreCallObject
            {
                TheClient = myClient,
                Server = theServerInfo
            };
            myClient.BeginGetCustomTre(theServerInfo.ServerId, new AsyncCallback(FinishedGettingTRE), theObject);

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.GettingCustomTre, true));
            }
        }
        #endregion

        #region Private Methods
        private void FinishedGettingTRE(IAsyncResult theResult)
        {
           
            CustomeTreCallObject theObject =  theResult.AsyncState as CustomeTreCallObject;
            List<LauncherData.CustomTre> lstCustomTres = theObject.TheClient.EndGetCustomTre(theResult);

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.Found + " " + lstCustomTres.Count.ToString(), false));
                PatchStepFired(this, new PatchingEventArgs(myVariables.CustomServerDir, true));
            }

            
            //does our directory even exist?
            if (!Directory.Exists(SWGANHPAth + theObject.Server.SafeFolderName))
            {
                if (PatchStepFired != null)
                {
                    PatchStepFired(this, new PatchingEventArgs(myVariables.NotFound, false));
                }
                CopyRequiredFiles(theObject.Server);
            }
            else
            {
                if (PatchStepFired != null)
                {
                    PatchStepFired(this, new PatchingEventArgs(myVariables.Found, false));
                }
            }

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.ValidatingFiles, true));
            }
            //now to check the files are correctly patched
            
            IniFiles myFiles = new IniFiles(SWGANHPAth + "\\swg2uu_live.cfg");
            List<KeyValuePair<string,string>> lstStandardFiles = myFiles.GetSectionValuesAsList("SharedFile");            
           

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.Found + " " + lstStandardFiles.Count.ToString(), false));
            }
            
            bool blnSuccess = true;
            foreach (KeyValuePair<string,string> thePair in lstStandardFiles)
            {
                //prepare the filename
                string strFileName = thePair.Value.Substring(1, thePair.Value.Length - 2);

                //is it a file?
                if (File.Exists(strFileName))
                {
                    using (FileStream fileIn = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                    {
                        byte[] arMD5Bytes = hashmd5.ComputeHash(fileIn);

                        //find the file in the Standard List
                        StandardTre theCurrentFile = myUserPreferences.CachedStandardTre.Find(st => st.Filename.Equals(Path.GetFileName(strFileName), StringComparison.InvariantCultureIgnoreCase));

                        if (theCurrentFile != null)
                        {
                            if (PatchStepFired != null)
                            {
                                PatchStepFired(this, new PatchingEventArgs(myVariables.Validating + " " + Path.GetFileName(strFileName), true));

                                if (BitConverter.ToString(arMD5Bytes).Replace("-", "") == theCurrentFile.MD5Hash)
                                {
                                    PatchStepFired(this, new PatchingEventArgs(myVariables.Passed, false));
                                }
                                else
                                {
                                    PatchStepFired(this, new PatchingEventArgs(myVariables.Failed, false));

                                    //something failed
                                    blnSuccess = false;
                                }
                            }                           
                        }
                    }
                }
            }

            if (PatchFunctionComplete != null)
            {
                string strPatchCompleteMessage = myVariables.ClientUpToDate;
                if(!blnSuccess)
                {
                    strPatchCompleteMessage = myVariables.ClientOutOfDate + Environment.NewLine + myVariables.PatchFromSWG;
                }
                PatchFunctionComplete(this, new PatchFunctionCompleteEventArgs(strPatchCompleteMessage, blnSuccess));
            }
        }

        private void CopyRequiredFiles(ServerInfo theServer)
        {
            //copy the files require to run the game

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.CreatingDirectory + " " + SWGANHPAth + theServer.SafeFolderName, true));
            }
            //make the directory
            if (!Directory.Exists(SWGANHPAth + theServer.SafeFolderName))
            {
                Directory.CreateDirectory(SWGANHPAth + theServer.SafeFolderName);
            }

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.Created, false));
                PatchStepFired(this, new PatchingEventArgs(myVariables.CopyingSoundSystem, true));
            }

            //copy the miles directory across
            Directory.CreateDirectory(SWGANHPAth + theServer.SafeFolderName + "\\miles");

            if (Directory.Exists(SWGANHPAth + "\\miles"))
            {
                foreach (string theFileName in Directory.GetFiles(SWGANHPAth + "\\miles"))
                {
                    File.Copy(theFileName, SWGANHPAth + theServer.SafeFolderName + "\\miles\\" + Path.GetFileName(theFileName));
                }
            }

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.Complete, false));
                PatchStepFired(this, new PatchingEventArgs(myVariables.CopyingBinaries, true));
            }

            //copy the raw files in there

            //main executable
            File.Copy(SWGANHPAth + "swganh.exe", SWGANHPAth + theServer.SafeFolderName+ "\\swganh.exe");

            //related DLLS            
            File.Copy(SWGANHPAth + "dpvs.dll", SWGANHPAth + theServer.SafeFolderName + "\\dpvs.dll");
            File.Copy(SWGANHPAth + "Mss32.dll", SWGANHPAth + theServer.SafeFolderName + "\\Mss32.dll");
            File.Copy(SWGANHPAth + "s205_r.dll", SWGANHPAth + theServer.SafeFolderName + "\\s205_r.dll");
            File.Copy(SWGANHPAth + "s206_r.dll", SWGANHPAth + theServer.SafeFolderName + "\\s206_r.dll");
            File.Copy(SWGANHPAth + "s207_r.dll", SWGANHPAth + theServer.SafeFolderName + "\\s207_r.dll");

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.Complete, false));
                PatchStepFired(this, new PatchingEventArgs(myVariables.CopyConfigFiles, true));
            }

            try
            {

                if (File.Exists(SWGANHPAth + "swg2uu_machineoptions.iff"))
                {
                    //one IFF
                    File.Copy(SWGANHPAth + "swg2uu_machineoptions.iff", SWGANHPAth + theServer.SafeFolderName + "\\swg2uu_machineoptions.iff");
                }
                //the pre-load CFG
                //File.Copy(SWGANHPAth + "preload.cfg", SWGANHPAth + theServer.ServerId.ToString() + "\\preload.cfg");

                //craft the login one to match
                File.Copy(SWGANHPAth + "swg2uu_login.cfg", SWGANHPAth + theServer.SafeFolderName + "\\swg2uu_login.cfg");

                if (PatchStepFired != null)
                {
                    PatchStepFired(this, new PatchingEventArgs(myVariables.Complete, false));
                    PatchStepFired(this, new PatchingEventArgs(myVariables.SettingServerAddress, true));
                }

                IniFiles myFiles = new IniFiles(SWGANHPAth + theServer.SafeFolderName + "\\swg2uu_login.cfg");
                myFiles.WriteValue("ClientGame", "loginServerPort0", theServer.Port);
                myFiles.WriteValue("ClientGame", "loginServerAddress0", theServer.Address);

                if (PatchStepFired != null)
                {
                    PatchStepFired(this, new PatchingEventArgs(myVariables.Complete, false));
                    PatchStepFired(this, new PatchingEventArgs(myVariables.SetupLocalConfig, true));
                }

                //and make a main cfg which points to the central config files
                //and our local login
                TextWriter twOut = new StreamWriter(SWGANHPAth + theServer.SafeFolderName + "\\swg2uu.cfg");
                twOut.WriteLine(".include \"swg2uu_login.cfg\"");
                twOut.WriteLine(".include \"..\\swg2uu_live.cfg\"");
                twOut.WriteLine(".include \"..\\swg2uu_preload.cfg\"");
                twOut.WriteLine(".include \"..\\swg2uu_opt.cfg\"");
                twOut.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (PatchStepFired != null)
            {
                PatchStepFired(this, new PatchingEventArgs(myVariables.Complete, false));
            }
        }
        #endregion
    }
}
