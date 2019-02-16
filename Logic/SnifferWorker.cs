using NetworkSnifferLib.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkSnifferLib.Logic
{
    public class SnifferWorker
    {
        public event NewPacketEventHandler NewPacket;
        public delegate void NewPacketEventHandler(SnifferPacket p);

        private const int BUFFER_MAX = 65535;
        private const int IOC_VENDOR = 0x18000000;
        private const int IOC_IN = -2147483648; //0x80000000; /* copy in parameters */
        private const int SIO_RCVALL = IOC_IN | IOC_VENDOR | 1;

        private byte[] buffer;
        private IPAddress ip;
        private Socket socket;
        private SnifferSettings settings;

        public SnifferWorker(SnifferSettings _settings)
        {
            settings = _settings;

            buffer = new byte[BUFFER_MAX];
        }

        public void start(IPAddress _ip)
        {
            ip = _ip;

            try
            {
                if (socket != null) {
                    throw new Exception("Socket Already Listen Please Destroy Listener");
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                socket.Bind(new IPEndPoint(ip, 0));
                socket.IOControl(SIO_RCVALL, BitConverter.GetBytes(1), null);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), null);
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void destroy() {
            if (socket != null) {
                socket.Close();
                socket = null;
            }

        }

        private void onReceive(IAsyncResult ar)
        {
            try
            {
                int received = socket.EndReceive(ar);
                try
                {
                    if (socket != null)
                    {
                        byte[] raw = new byte[received];
                        Array.Copy(buffer, 0, raw, 0, received);
                        NewPacket(new SnifferPacket(raw));
                    }
                }
                catch { } // invalid packet; ignore
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
