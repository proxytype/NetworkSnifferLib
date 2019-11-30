using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using NetworkSnifferLib.Model;

namespace NetworkSnifferLib.Logic
{
    public class SnifferWorker
    {

        public TCPHeader Tcp;

        public int TotalPackages { set; get; }
        public int TotalTCPPackages { set; get; }
        public int TotalUDPPackages { set; get; }
        public string TypeIp { set; get; }

        private const int BUFFER_MAX = 4096;

        private byte[] buffer;
        private IPAddress ip;
        private Socket socket;

        public List<IPHeader> PacketListReceived;
        public List<string> PacketLog;

        private bool ExportPacketData { get; set; }

        //Construtor da classe que inicializa as variáveis.
        public SnifferWorker(bool _exportPacketData)
        {
            ExportPacketData = _exportPacketData;
            buffer = new byte[BUFFER_MAX];
            PacketListReceived = new List<IPHeader>();
            PacketLog = new List<string>();
            TotalPackages = 0;
            TotalTCPPackages = 0;
            TotalUDPPackages = 0;
            TypeIp = "";
        }

        //Faz a obtenção dos pacotes
        public void Start(IPAddress _ip)
        {

            ip = _ip;
            byte[] byTrue = new byte[4] { 1, 0, 0, 0 };
            byte[] byOut = new byte[4] { 1, 0, 0, 0 };

            try
            {
                if (socket != null)
                {
                    throw new Exception("Socket Already Listen Please Destroy Listener");
                }


                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                socket.Bind(new IPEndPoint(ip, 0));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                socket.IOControl(IOControlCode.ReceiveAll, byTrue, byOut);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (Exception ex)
            {

                this.Destroy();                
                throw ex;
            }


        }

        //Fecha o socket se o mesmo não for nulo
        public void Destroy()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }

        }

        //Processa, analisa os pacotes recebidos, e armazena as informações em uma lista
        private void ParseData(byte[] byteData, int received)
        {
            IPHeader ipHeader = new IPHeader(byteData, received);
            ipHeader.ListIndex = PacketListReceived.Count;
            PacketListReceived.Add(ipHeader);
            TypeIp = ipHeader.Version;
            TotalPackages++;

            if (ipHeader.ProtocolType == Protocol.TCP)
            {
                TotalTCPPackages++;
                Tcp = new TCPHeader(byteData, received);
            }
            else if (ipHeader.ProtocolType == Protocol.UDP)
            {
                TotalUDPPackages++;
                Tcp = null;
            }

            PacketLog.Add(ipHeader.GetIpLog(ipHeader.ProtocolType, Tcp));
        }

        //Destrava os ponteiros
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int received = socket.EndReceive(ar);

                ParseData(buffer, received);

                if (socket != null)
                {
                    buffer = new byte[BUFFER_MAX];

                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
