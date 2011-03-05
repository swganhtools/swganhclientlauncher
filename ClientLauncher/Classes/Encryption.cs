using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Security.Cryptography;

namespace ClientLauncher
{
    public class LauncherEncryption
    {
        public LauncherEncryption()
        {
           
        }

        private byte[] arKeyBytes;      

        public byte[] BaseKey
        {
            get
            {
                if (arKeyBytes == null)
                {
                    string strSerial = "";
                    ManagementObjectSearcher moSearch = new ManagementObjectSearcher("select SerialNumber from Win32_BaseBoard");
                    ManagementObjectCollection moObjects = moSearch.Get();

                    foreach (ManagementObject moSerial in moObjects)
                    {
                        strSerial += moSerial["SerialNumber"].ToString();
                    }

                    arKeyBytes = UTF8Encoding.UTF8.GetBytes(strSerial);
                }

                return arKeyBytes;
            }
        }

        private enum CryptographicDirection
        {
            Encrypt,
            Decrypt
        }


        public byte[] MD5Bytes(byte[] arInputBytes)
        {
            byte[] arReturnBytes;
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

            try
            {
                arReturnBytes = hashmd5.ComputeHash(arInputBytes);
            }
            finally
            {
                hashmd5.Clear();
            }

            return arReturnBytes;
        }

        /// <summary>
        /// Compares the bytes in two arrays to check for equality
        /// </summary>
        /// <param name="arFirstArray"></param>
        /// <param name="arSecondArray"></param>
        /// <returns></returns>
        public bool CompareArrays(byte[] arFirstArray, byte[] arSecondArray)
        {
            bool blnIdentical = true;
            //are they the same length?
            if (arFirstArray.Length != arSecondArray.Length)
            {
                blnIdentical = false;
            }
            else
            {
                //check the contents
                int iByte = 0;
                do
                {
                    blnIdentical = (arFirstArray[iByte] == arSecondArray[iByte]);
                } while (blnIdentical && iByte < arFirstArray.Length);
            }

            return blnIdentical;
        }


        private byte[] EncryptDecrypt(byte[] arCryptoBytes, CryptographicDirection theDirection)
        {
            byte[] arOutputBytes;

            //get our cryptographic providers out
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider my3DES = new TripleDESCryptoServiceProvider();
            ICryptoTransform myTransform = null;

            try
            {
                //hash the key
                my3DES.Key = hashmd5.ComputeHash(BaseKey);
                my3DES.Mode = CipherMode.ECB;
                my3DES.Padding = PaddingMode.PKCS7;               
                
                switch(theDirection)
                {
                    case CryptographicDirection.Encrypt:
                        myTransform = my3DES.CreateEncryptor();
                        break;
                    case CryptographicDirection.Decrypt:
                        myTransform = my3DES.CreateDecryptor();
                        break;
                }
                
                arOutputBytes = myTransform.TransformFinalBlock(arCryptoBytes, 0, arCryptoBytes.Length);
            }
            finally
            {
                //clear down the objects
                //the are managed wrappers to unmanaged objects so best to dispose of them properly
                hashmd5.Clear();
                my3DES.Clear();
                myTransform.Dispose();
            }

            return arOutputBytes;
        }

        public byte[] Encrypt(byte[] arEncryptMe)
        {
            return EncryptDecrypt(arEncryptMe, CryptographicDirection.Encrypt);
        }

        public byte[] Decrypt(byte[] arDecryptMe)
        {
            return EncryptDecrypt(arDecryptMe, CryptographicDirection.Decrypt);
        }
    }
}
