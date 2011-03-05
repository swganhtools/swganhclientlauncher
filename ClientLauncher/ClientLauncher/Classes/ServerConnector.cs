using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace ClientLauncher
{
    public class ServerConnector
    {
        [DllImport("PacketHandler.dll")]
        static public extern UInt32 GenerateCrc(byte[] strChars, ushort nLength, uint nCRCSeed);

        private TextVariables myTextVariables;

        public ServerConnector(TextVariables theTextVariables)
        {
            myTextVariables = theTextVariables;
        }

        public string Message
        {
            get;
            private set;
        }

        private byte[] arCRCSeed;

        public bool CreateSessionKey(ServerInfo theServerInfo, string strUsername, string strPassword)
        {

            bool blnSuccess = false;
            //open a memory stream to hold our session packet
            MemoryStream msSessionOpen = new MemoryStream();
            msSessionOpen.Write(new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, 0, 6);

            //take 4 random digits from a guid
            Random myRandom = new Random((int)DateTime.Now.Ticks);
            byte[] arRandom = Guid.NewGuid().ToByteArray();
            for (int i = 0; i < 4; i++)
            {
                msSessionOpen.WriteByte(arRandom[myRandom.Next(0, 31)]);
            }

            //finish it off
            msSessionOpen.Write(new byte[] { 0x00, 0x00, 0x01, 0xf0 }, 0, 4);

            //and extract it's bytey goodness
            byte[] arSessionOpen = msSessionOpen.ToArray();
            msSessionOpen.Flush();
            msSessionOpen.Close();

            //make the client and connect it to the server
            UdpClient myClient = new UdpClient();
            //give it a ten second timeout
            myClient.Client.SendTimeout = 1000;
            myClient.Client.ReceiveTimeout = 1000;
            myClient.Connect(theServerInfo.Address, theServerInfo.Port);

            //and send it out session creation packet
            myClient.Send(arSessionOpen, arSessionOpen.Length);

            try
            {
                //what's the server saying?
                IPEndPoint rec = new IPEndPoint(IPAddress.Any, 0);
                byte[] arReceivedBytes = myClient.Receive(ref rec);

                //craft our special launcher packet

                //get the ol' CRC seed
                arCRCSeed = new byte[] { arReceivedBytes[6], arReceivedBytes[7], arReceivedBytes[8], arReceivedBytes[9] };
                Array.Reverse(arCRCSeed);
                uint myCRCSeed = BitConverter.ToUInt32(arCRCSeed, 0);

                //and use the luancher session opcode - Hoon :¬)
                byte[] arOpCode = new byte[] { 0x48, 0x6f, 0x6f, 0x6e };
                Array.Reverse(arOpCode);

                //make arrays for our login details
                byte[] arUserName = Encoding.ASCII.GetBytes(strUsername);
                byte[] arPassword = Encoding.ASCII.GetBytes(strPassword);

                //Aubrey's birthday
                byte[] arStationId = Encoding.ASCII.GetBytes("20090610-18:00");

                //and construct our luancher session packet
                MemoryStream msPacket = new MemoryStream();                
                msPacket.Write(new byte[] { 0x00, 0x00, 0x04, 0x00 }, 0, 4);
                msPacket.Write(arOpCode, 0, arOpCode.Length);
                byte[] arUsernameLength = BitConverter.GetBytes((short)arUserName.Length);
                msPacket.Write(arUsernameLength, 0, arUsernameLength.Length);
                msPacket.Write(arUserName, 0, arUserName.Length);
                byte[] arPasswordLength = BitConverter.GetBytes((short)arPassword.Length);
                msPacket.Write(arPasswordLength, 0, arPasswordLength.Length);
                msPacket.Write(arPassword, 0, arPassword.Length);
                byte[] arStartionIdLength = BitConverter.GetBytes((short)arStationId.Length);
                msPacket.Write(arStartionIdLength, 0, arStartionIdLength.Length);
                msPacket.Write(arStationId, 0, arStationId.Length);
                msPacket.WriteByte(0x00);
                byte[] arCompressMe = msPacket.ToArray();
                msPacket.Close();

                //encrypt it
                Encrypt(ref arCompressMe, arCRCSeed);

                //and put it all together
                MemoryStream msFinalPacket = new MemoryStream();
                msFinalPacket.Write(new byte[] { 0x00, 0x09 }, 0, 2);
                msFinalPacket.Write(arCompressMe, 0, arCompressMe.Length);
                //msFinalPacket.WriteByte(0x00);

                //now to generate the CRC for the packet
                byte[] arFinalPacket = msFinalPacket.ToArray();
                UInt32 myCRC = GenerateCrc(arFinalPacket, (ushort)arFinalPacket.Length, myCRCSeed);
                byte[] arCRC = BitConverter.GetBytes(myCRC);
                Array.Reverse(arCRC);

                //and add this on to the end
                msFinalPacket.Write(arCRC, 2, arCRC.Length - 2);
                arFinalPacket = msFinalPacket.ToArray();
                msFinalPacket.Close();

                //send it all up the wire
                myClient.Send(arFinalPacket, arFinalPacket.Length);  
             
                //and wait to recieve
                myClient.BeginReceive(new AsyncCallback(PacketRecieve), myClient);

                blnSuccess = true;

            }
            catch (SocketException exSocket)
            {
                //Session Times out
                if (exSocket.ErrorCode == 10060)
                {
                    Message = myTextVariables.ConnectionTimeout;
                }
                else
                {
                    Message = exSocket.Message;
                }

                blnSuccess = false;

                myClient.Close();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                blnSuccess = false;
                myClient.Close();
            }

            return blnSuccess;
        }

        private int nIterations = 0;
        List<byte[]> lstPackets = new List<byte[]>();
        private void PacketRecieve(IAsyncResult theResult)
        {
            string strMessage = "";
            bool blnSuccess = false;
            string strSessionKey = "";


            IPEndPoint rec = new IPEndPoint(IPAddress.Any, 0);
            UdpClient myClient = theResult.AsyncState as UdpClient;
            lstPackets.Add(myClient.EndReceive(theResult, ref rec));

            nIterations++;
            //and see what else comes back 

            if ((lstPackets.Count < 3) && (nIterations < 4))
            {
                //need to check if this server supports the launcher
                if ((lstPackets.Count == 2) && (lstPackets[1][1] == 0x05))
                {
                    //disconnect packet
                    strMessage = myTextVariables.LauncherNotSupportedOne + Environment.NewLine + myTextVariables.LauncherNotSupportedTwo;
                    blnSuccess = false;
                }
                else
                {
                    myClient.BeginReceive(new AsyncCallback(PacketRecieve), myClient);
                    return;
                }
            }
            else if (lstPackets.Count == 3)
            {
                //0x74654e2e

                //decrypt the middle packet
                byte[] arDecryptMe = lstPackets[1];
                Decrypt(ref arDecryptMe, arCRCSeed);

                //is it our launcher opcode for returing the session key
                byte[] arOpcode = new byte[] { arDecryptMe[6], arDecryptMe[7], arDecryptMe[8], arDecryptMe[9] };
                Array.Reverse(arOpcode);
                uint myOpCode = BitConverter.ToUInt32(arOpcode, 0);

                if (myOpCode == 0x74654e2e)
                {
                    //get our session key
                    MemoryStream msSessionKey = new MemoryStream();
                    msSessionKey.Write(arDecryptMe, 0x10, 32);
                    strSessionKey = Encoding.ASCII.GetString(msSessionKey.ToArray());
                    msSessionKey.Flush();
                    msSessionKey.Close();
                    strMessage = "Success";
                    blnSuccess = true;
                }
                else
                {
                    strMessage = myTextVariables.CredentialsIncorrect;
                    blnSuccess = false;
                }
            }
            else
            {
                strMessage = myTextVariables.CredentialsIncorrect;
                blnSuccess = false;
            }

            myClient.Close();

            if (PacketReceived != null)
            {
                PacketReceived(this, new PacketsReceivedEventArgs(blnSuccess, strMessage, strSessionKey));
            }

        }

        public event EventHandler<PacketsReceivedEventArgs> PacketReceived;

        public class PacketsReceivedEventArgs : EventArgs
        {
            public bool Success
            {
                get;
                private set;
            }

            public string Message
            {
                get;
                private set;
            }

            public string SessionKey
            {
                get;
                private set;
            }

            public PacketsReceivedEventArgs(bool blnSuccess, string strMessage, string strSessionKey)
            {
                this.Success = blnSuccess;
                this.Message = strMessage;
                this.SessionKey = strSessionKey;
            }
        }



        private static void Decrypt(ref byte[] arDecryptMe, byte[] arCRCBytes)
        {
            //how many whole blocks?
            //how many block have we here?
            //length of packet data
            //skipping the opcode
            //and the crc
            //and then multiple of 4 bytes in the remainder
            int nBlocks = (int)Math.Floor(Convert.ToDouble((arDecryptMe.Length - 4) / 4));

            if (nBlocks == 0)
            {
                return;
            }

            //and odd bytes?
            int nOddBytes = (arDecryptMe.Length - 4) % 4;

            if (nOddBytes > 0)
            {
                //work these in from the end
                int nOddOffset = arDecryptMe.Length - 2 - nOddBytes;

                //hold the byte to XOR with
                byte xorMe = arDecryptMe[nOddOffset - 4];

                //and xor these
                for (int i = nOddOffset; i < arDecryptMe.Length - 2; i++)
                {
                    arDecryptMe[i] = Convert.ToByte(arDecryptMe[i] ^ xorMe);
                }
            }

            //now start the blocks
            byte[] arCurrentCRC = new byte[4];
            int nStart = arDecryptMe.Length - 2 - nOddBytes - 1;
            int nOffset = 0;
            for (int iBlocks = 0; iBlocks < nBlocks - 1; iBlocks++)
            {
                //offset for this cycle
                nOffset = nStart - (iBlocks * 4);

                for (int i = nOffset; i > nOffset - 4; i--)
                {
                    arDecryptMe[i] = Convert.ToByte(arDecryptMe[i] ^ arDecryptMe[i - 4]);
                }
            }

            //now the first set we have to use the passed CRC for since there are no 'previous' bytes
            arDecryptMe[5] = Convert.ToByte(arDecryptMe[5] ^ arCRCBytes[3]);
            arDecryptMe[4] = Convert.ToByte(arDecryptMe[4] ^ arCRCBytes[2]);
            arDecryptMe[3] = Convert.ToByte(arDecryptMe[3] ^ arCRCBytes[1]);
            arDecryptMe[2] = Convert.ToByte(arDecryptMe[2] ^ arCRCBytes[0]);

        }        

        private static void Encrypt(ref byte[] arEncryptMe, byte[] arCRCBytes)
        {
            //start at the third byte
            //skipping the OpCode
            int nStart = 0;

            byte[] arCurrentCrc = new byte[4];

            //copy the CRC from the param
            arCurrentCrc[0] = arCRCBytes[0];
            arCurrentCrc[1] = arCRCBytes[1];
            arCurrentCrc[2] = arCRCBytes[2];
            arCurrentCrc[3] = arCRCBytes[3];

            //how many block have we here?
            //length of packet data
            //skipping the opcode
            //and the crc
            //and then multiple of 4 bytes in the remainder
            int nBlocks = (int)Math.Ceiling(Convert.ToDouble(arEncryptMe.Length / 4));
            int nOffset = 0;
            int nCRC = 0;
            for (int iBlocks = 0; iBlocks < nBlocks; iBlocks++)
            {
                //offest for this cycle
                nOffset = nStart + (iBlocks * 4);

                for (int i = nOffset; i < nOffset + 4 && i < arEncryptMe.Length - 1; i++)
                {
                    //XOR the byte
                    arEncryptMe[i] = Convert.ToByte(arEncryptMe[i] ^ arCurrentCrc[nCRC]);

                    //store that byte for the next round
                    arCurrentCrc[nCRC] = arEncryptMe[i];

                    //and move on to the next byte in the CRC array
                    nCRC++;
                }

                nCRC = 0;
            }

            //clock on the offset
            nOffset += 4;

            //any remainders?
            while (nOffset < arEncryptMe.Length)
            {
                arEncryptMe[nOffset] = Convert.ToByte(arEncryptMe[nOffset] ^ arCurrentCrc[nCRC]);
                nOffset++;
            }
        }
    }
}
