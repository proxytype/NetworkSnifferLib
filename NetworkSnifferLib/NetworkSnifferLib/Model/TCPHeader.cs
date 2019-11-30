using System;
using System.IO;
using System.Net;

namespace NetworkSnifferLib.Model
{
    public class TCPHeader
    {

        private uint uiSequenceNumber;   
        private uint uiAcknowledgementNumber;
        private ushort usDataOffsetAndFlags;
        private ushort usWindow;
        private ushort usUrgentPointer;

        public TCPHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                uiSequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                uiAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                usDataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usWindow = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            }
            catch (Exception ex)
            {
                throw new Exception("TCPHeader constructor exception: "+ex);
            }
        }
        //Dados do cabeçalho
        public string SequenceNumber => uiSequenceNumber.ToString(); //Retorna o número de sequência
        public string WindowSize => usWindow.ToString();  //Retorna o window size
        public string AcknowledgementNumber => uiAcknowledgementNumber.ToString(); //Retorna o número de reconhecimento
        public string UrgentPointer => usUrgentPointer.ToString(); //Retorna o Urgent Pointer
        public string Flags => usDataOffsetAndFlags.ToString(); //Retorna a flag
    }
}