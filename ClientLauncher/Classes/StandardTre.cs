using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLauncher
{
    [Serializable]
    public class StandardTre
    {
        private string strFilename;
        private string strMD5Hash;

        public string Filename
        {
            get
            {
                return strFilename;
            }
            set
            {
                strFilename = value;
            }
        }

        public string MD5Hash
        {
            get
            {
                return strMD5Hash;
            }
            set
            {
                strMD5Hash = value;
            }
        }
    }
}