using System;
using System.IO;
using System.Net;

namespace NetworkSnifferLib.Model
{
    public class IPHeader
    {

        private byte byVersionAndHeaderLength;
        private byte byDifferentiatedServices;
        private ushort usTotalLength;
        private ushort usIdentification;
        private ushort usFlagsAndOffset;
        private byte byTTL;
        private byte byProtocol;
        private short sChecksum;
        private uint uiSourceIPAddress;
        private uint uiDestinationIPAddress;
        private byte byHeaderLength;
        private byte[] byIPData = new byte[4096];
        public int ListIndex = 0;

        public IPHeader(byte[] byBuffer, int nReceived)
        {

            try
            {

                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                byVersionAndHeaderLength = binaryReader.ReadByte();
                byDifferentiatedServices = binaryReader.ReadByte();
                usTotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usIdentification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usFlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                byTTL = binaryReader.ReadByte();
                byProtocol = binaryReader.ReadByte();
                sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                uiSourceIPAddress = (uint)(binaryReader.ReadInt32());
                uiDestinationIPAddress = (uint)(binaryReader.ReadInt32());

                byHeaderLength = byVersionAndHeaderLength;
                byHeaderLength <<= 4;
                byHeaderLength >>= 4;
                byHeaderLength *= 4;

                Array.Copy(byBuffer, byHeaderLength, byIPData, 0, usTotalLength - byHeaderLength);
            }
            catch (Exception ex)
            {
                throw new Exception("IPHeader constructor Exception:"+ex);
            }
        }

        //Retorna a versão do IP
        public string Version
        {
            get
            {

                switch (byVersionAndHeaderLength >> 4)
                {
                    case 4:
                        return "IP v4";
                    case 6:
                        return "IP v6";
                    default:
                        return "Unknown";
                }

            }
        }

        public string HeaderLength => byHeaderLength.ToString(); //Retorna o tamanho do cabeçalho.
        public ushort MessageLength => (ushort) (usTotalLength - byHeaderLength); //Retorna o tamanho da mensagem.
        public string DifferentiatedServices => string.Format("0x{0:x2} ({1})", byDifferentiatedServices, byDifferentiatedServices); //Retorna os serviços diferenciados da rede
        public string TTL => byTTL.ToString(); //Retorna o valor do TTL
        public string Checksum => string.Format("0x{0:x2}", sChecksum); //Retorna o valor do Checksum
        public IPAddress SourceAddress => new IPAddress(uiSourceIPAddress); //Retorna o endereço fonte
        public IPAddress DestinationAddress => new IPAddress(uiDestinationIPAddress); //Retorna o endereço de destino
        public string TotalLength => usTotalLength.ToString(); //Retorna o comprimento total
        public string Identification => usIdentification.ToString(); //Retorna a identificação

        //Retorna as flags
        public string Flags
        {
            get
            {

                switch (usFlagsAndOffset >> 13)
                {
                    case 2:
                        return "DF";
                    case 1:
                        return "MF";
                    default:
                        return (usFlagsAndOffset >> 13).ToString();
                }

            }
        }

        //Retorna o deslocamento de fragmentação
        public string FragmentationOffset
        {
            get
            {
                int nOffset = usFlagsAndOffset << 3;
                nOffset >>= 3;

                return nOffset.ToString();
            }
        }

        //Retorna o tipo de protocolo
        public Protocol ProtocolType
        {
            get
            {
                if (byProtocol == 6) 
                {
                    return Protocol.TCP;
                }
                
                if (byProtocol == 17)
                {
                    return Protocol.UDP;
                }

                return Protocol.Unknown;
                
            }
        }

        //Retorna informações do log obtidas no IP
        public string GetIpLog(Protocol type, TCPHeader tcp)
        {
            string body = "";

            body += Identification + " . From  " + SourceAddress + " to " + DestinationAddress+"\n";
            body += "Protocol: " + (type == Protocol.TCP ? "TCP" : "UDP")+" from "+"\n";
            if (type == Protocol.TCP && tcp != null)
            {
                body += "Window size: "+tcp.WindowSize+(tcp.UrgentPointer != "" ? " ,Urgent Pointer: "+ tcp.UrgentPointer : "")+"\n";
                body += "Sequence Number: " + tcp.SequenceNumber + " ,Acknowledgement Number: "+tcp.AcknowledgementNumber+" ,Flags: "+tcp.Flags+"\n";
            }
            body += " TTL: " + TTL+ " ,Checksum: " + Checksum + " ,DiffServ: " + DifferentiatedServices + " ,Fragmentation: " + FragmentationOffset + Flags+"\n";
            body += " Total Length: " + TotalLength + " Header length: "+HeaderLength+" Message Length: "+MessageLength+ "\n";

            body += "\n";
            return body;
        }
    }
}