using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientLauncher.LauncherData;
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ClientLauncher
{
    public class ApplicationUpdates
    {
        public ApplicationUpdates(LauncherData.LauncherDataClient myClient)
        {
            _WCFClient = myClient;
        }
        LauncherData.LauncherDataClient _WCFClient;
        private byte[] arFileBytes;
        private long nBytesRead;

        public static string CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            }
        }
        public List<LauncherVersion> UpdateAvailable(Dispatcher myDispatcher)
        {
            List<LauncherData.LauncherVersion> lstVersions = new List<LauncherVersion>();
            try
            {
                //check the service to see if there are any updates available
                lstVersions = _WCFClient.GetLatestVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
            }
            catch
            {
                //MessageBox.Show("Service unavailable","SWG:ANH", MessageBoxButtons.OK);
                //myDispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
                //{
                //    App.Current.Shutdown();

                //}), System.Windows.Threading.DispatcherPriority.Normal);  

                
            }

            return lstVersions;
        }

        public event EventHandler<DownloadCompleteEventArgs> OnDownloadComplete;
        public class DownloadCompleteEventArgs : EventArgs
        {
            public string ExecutablePath
            {
                get;
                private set;
            }

            public DownloadCompleteEventArgs(string strExecutablePath)
            {
                this.ExecutablePath = strExecutablePath;
            }
        }


        public event EventHandler<DownloadProgressEventArgs> OnDownloadProgress;
        public class DownloadProgressEventArgs : EventArgs
        {
            public long Downloaded
            {
                get;
                private set;
            }

            public long Total
            {
                get;
                private set;
            }

            public string PercentComplete
            {
                get
                {
                    string strPercent = "0%";
                    if (Total > 0)
                    {
                        strPercent = String.Format("{0:##.00′%", Downloaded / Total);
                    }

                    return strPercent;                    
                }
            }

            public Double Percent
            {
                get
                {
                    Double dblComplete = 0;

                    if (Total > 0)
                    {
                        dblComplete = (double)Downloaded / (double)Total;
                    }

                    return dblComplete;
                }
            }

            public DownloadProgressEventArgs(long nDownloaded, long nTotal)
            {
                this.Downloaded = nDownloaded;
                this.Total = nTotal;
            }
        }

        public class DownloadRequest
        {
            public WebRequest Request                
            {
                get;
                set;
            }

            public LauncherData.LauncherVersion Version
            {
                get;
                set;
            }
        }

        public class DownloadResponse
        {
            public Stream TheStream
            {
                get;
                set;
            }

            public LauncherData.LauncherVersion Version
            {
                get;
                set;
            }
        }

        public void UpdateToLatestVersion(Dispatcher myDispatcher, LauncherData.LauncherVersion theVersion)
        {
            myDispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                //open a webrequest
                WebRequest req = WebRequest.Create("http://swganh.hooni.us" + theVersion.Location);
                req.Method = "GET";
                
                //wrap our object togethers
                DownloadRequest theRequest = new DownloadRequest
                {
                    Request = req,
                    Version = theVersion
                };

                //and initialise an array to hold our bytes[]
                arFileBytes = new byte[theVersion.FileSize];

                //and a counter to record the download progress
                nBytesRead = 0;

                //start the download
                req.BeginGetResponse(new AsyncCallback(DownloadStarted), theRequest);                
                
            }), System.Windows.Threading.DispatcherPriority.Normal);             
        }

        private void DownloadStarted(IAsyncResult theResult)
        {
            //get our wrapper back out
            DownloadRequest theRequest = theResult.AsyncState as DownloadRequest;

            //notify anyone that might be listening that we have commence our download
            if (OnDownloadProgress != null)
            {
                OnDownloadProgress(this, new DownloadProgressEventArgs(0, theRequest.Version.FileSize));
            }

            //get the response
            WebResponse theResponse = theRequest.Request.EndGetResponse(theResult);

            //grab the stream
            Stream theStream = theResponse.GetResponseStream();

            //wrap it up so we can pass it along
            DownloadResponse TheResponse = new DownloadResponse
            {
                TheStream = theStream,
                Version = theRequest.Version
            };

            //now to start reading from it
            theStream.BeginRead(arFileBytes, 0, 100, new AsyncCallback(ReadingStream), TheResponse);

        }

        private void ReadingStream(IAsyncResult theResult)
        {
            //get out wrapper out again
            DownloadResponse theResponse = theResult.AsyncState as DownloadResponse;
             
            //how many bytes read?
            nBytesRead += theResponse.TheStream.EndRead(theResult);

            //notify our users
            if (OnDownloadProgress != null)
            {
                OnDownloadProgress(this, new DownloadProgressEventArgs(nBytesRead, theResponse.Version.FileSize));
            }

            if (nBytesRead == theResponse.Version.FileSize)
            {
                //completed

                //close the stream
                theResponse.TheStream.Flush();
                theResponse.TheStream.Close();
                
                DownloadComplete(theResponse.Version, arFileBytes);
            }
            else
            {
                if (theResponse.Version.FileSize - nBytesRead >= 100)
                {
                    //read the next chunk
                    theResponse.TheStream.BeginRead(arFileBytes, (int)nBytesRead, 100, new AsyncCallback(ReadingStream), theResponse);
                }
                else
                {
                    //read the last chunk
                    theResponse.TheStream.BeginRead(arFileBytes, (int)nBytesRead, (int)(theResponse.Version.FileSize - nBytesRead), new AsyncCallback(ReadingStream), theResponse);
                }
            }
        }

        private void DownloadComplete(LauncherData.LauncherVersion theVersion, byte[] arFileBytes)
        {
            App.Current.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                string strUpdateDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Updates";

                string strFilename = Path.GetFileName(theVersion.Location);

                if (!Directory.Exists(strUpdateDirectory))
                {
                    Directory.CreateDirectory(strUpdateDirectory);
                }
                if (File.Exists(strUpdateDirectory + Path.DirectorySeparatorChar + strFilename))
                {
                    File.Delete(strUpdateDirectory + Path.DirectorySeparatorChar + strFilename);
                }
                FileStream outFile = new FileStream(strUpdateDirectory + Path.DirectorySeparatorChar + strFilename, FileMode.CreateNew, FileAccess.Write);
                BinaryWriter writeME = new BinaryWriter(outFile);
                writeME.Write(arFileBytes);
                writeME.Close();
                outFile.Close();
                if (OnDownloadComplete != null)
                {
                    OnDownloadComplete(this, new DownloadCompleteEventArgs(strUpdateDirectory + Path.DirectorySeparatorChar + strFilename));                
                }

            }), System.Windows.Threading.DispatcherPriority.Normal);    
        }

    }
}
