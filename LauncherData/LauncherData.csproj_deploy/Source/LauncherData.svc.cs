using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Hosting;
namespace LauncherData
{
    // NOTE: If you change the class name "LauncherData" here, you must also update the reference to "LauncherData" in Web.config.
    
    public class LauncherData : ILauncherData
    {

        #region ILauncherData Members

        List<ServerInfo> ILauncherData.GetServers()
        {
            LauncherDataContext dc = new LauncherDataContext();

            //just return a list of all the servers            
            var allServers = from ts in dc.tServers
                             orderby ts.Population descending
                             select new ServerInfo
                             {
                                 ServerId = ts.ServerId,
                                 ServerName = ts.Name,
                                 Address = ts.Address,
                                 CharsCreated = ts.CharsCreated,
                                 Population = ts.Population,
                                 Description = ts.Description,
                                 LastUpdated = ts.LastUpdated,
                                 Port = ts.Port,
                                 RSSFeedUrl = ts.NewsUrl,
                                 LauncherPort = ts.LauncherPort
                             };

            return allServers.ToList();
        }

        void ILauncherData.UpdateServer(ServerInfo theServerInfo)
        {
            LauncherDataContext dc = new LauncherDataContext();

            //find the server
            try
            {
                tServer theServer = (from ts in dc.tServers
                                     where ts.ServerId == theServerInfo.ServerId
                                     select ts).Single();

                //update the details
                theServer.Address = theServerInfo.Address;
                theServer.CharsCreated = theServerInfo.CharsCreated;
                theServer.Description = theServerInfo.Description;
                theServer.LastUpdated = DateTime.Now;
                theServer.Name = theServerInfo.ServerName;
                theServer.NewsUrl = theServerInfo.RSSFeedUrl;
                theServer.Population = theServerInfo.Population;
                theServer.Port = theServerInfo.Port;
                theServer.LauncherPort = theServerInfo.LauncherPort;
            }
            catch
            {
                //make a new one then
                tServer theNewServer = new tServer();
                theNewServer.Address = theServerInfo.Address;
                theNewServer.CharsCreated = theServerInfo.CharsCreated;
                theNewServer.Description = theServerInfo.Description;
                theNewServer.LastUpdated = DateTime.Now;
                theNewServer.Name = theServerInfo.ServerName;
                theNewServer.NewsUrl = theServerInfo.RSSFeedUrl;
                theNewServer.Population = theServerInfo.Population;
                theNewServer.Port = theServerInfo.Port;
                theNewServer.LauncherPort = theServerInfo.LauncherPort;
                dc.tServers.InsertOnSubmit(theNewServer);
            }

            dc.SubmitChanges();
        }

        List<LauncherVersion> ILauncherData.GetLatestVersion(string strCurrentVersion)
        {

            //just return the latest version
            LauncherDataContext dc = new LauncherDataContext();

            //grab the current version
            tLauncherVersion curVersion = (from tv in dc.tLauncherVersions
                                           where tv.VersionNumber == strCurrentVersion
                                           select tv).Single();

            var theLatestsVersion = from tv in dc.tLauncherVersions
                                    where tv.DateCreated > curVersion.DateCreated
                                    orderby tv.DateCreated descending
                                    select new LauncherVersion
                                    {
                                        DateCreated = tv.DateCreated,
                                        PatchNotes = tv.PatchNotes,
                                        VersionId = tv.VersionId,
                                        VersionNumber = tv.VersionNumber,
                                        Location = "/launcher/" + tv.VersionNumber + "/swganhlauncher.msi"
                                    };

            List<LauncherVersion> lstLatestVersions = theLatestsVersion.ToList();

            if (lstLatestVersions.Count > 0)
            {
                //update the file size on the first one           
                FileStream inFile = new FileStream(HostingEnvironment.MapPath("~" + lstLatestVersions[0].Location), FileMode.Open);
                lstLatestVersions[0].FileSize = inFile.Length;
                inFile.Close();
            }

            return lstLatestVersions;

        }

        LatestFile ILauncherData.GetLatestExecutable()
        {
            LauncherDataContext dc = new LauncherDataContext();
            tLauncherVersion theLatestsVersion = (from tv in dc.tLauncherVersions
                                                 orderby tv.DateCreated descending
                                                 select tv).First();

            FileStream inFile = new FileStream(HostingEnvironment.MapPath("~/" + theLatestsVersion.DownloadPath), FileMode.Open);
            byte[] arFileFiles = new byte[inFile.Length];
            inFile.Read(arFileFiles, 0, (int)inFile.Length);
            inFile.Close();

            LatestFile myLatestFile = new LatestFile
            {
                FileName = Path.GetFileName(theLatestsVersion.DownloadPath),
                FileBytes = arFileFiles
            };

            return myLatestFile;
        }

        List<CustomTre> ILauncherData.GetCustomTre(Guid guServerId)
        {
            LauncherDataContext dc = new LauncherDataContext();

            var customTreFiles = from ct in dc.tCustomTres
                                 where ct.ServerId == guServerId
                                 select new CustomTre
                                 {
                                     FileName = ct.Name,
                                     Uri = ct.Uri,
                                     MD5Hash = ct.MD5
                                 };

            return customTreFiles.ToList();
        }

        List<StandardTre> ILauncherData.GetStandardTre()
        {
            LauncherDataContext dc = new LauncherDataContext();

            var standardTreFiles = from sf in dc.tStandardTres
                                   select new StandardTre
                                   {
                                       Filename = sf.Filename,
                                       MD5Hash = sf.MD5
                                   };

            return standardTreFiles.ToList();
        }

        #endregion
    }
}
