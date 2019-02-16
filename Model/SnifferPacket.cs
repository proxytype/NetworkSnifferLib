using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkSnifferLib.Model
{
    public class SnifferPacket
    {
        //https://en.wikipedia.org/wiki/Type_of_service
        public enum TYPE_OF_SERVICE_PRECENDENCE
        {
            Routine = 0,
            Priority = 1,
            Immediate = 2,
            Flash = 3,
            FlashOverride = 4,
            CRITICECP = 5,
            InternetworkControl = 6,
            NetworkControl = 7
        }

        public enum TYPE_OF_SERVICE_DELAY
        {
            NormalDelay = 0,
            LowDelay = 1
        }

        public enum TYPE_OF_SERVICE_THROUGHPUT
        {
            NormalThroughput = 0,
            HighThroughput = 1
        }

        public enum TYPE_OF_SERVICE_RELIABLITIY
        {
            NormalReliability = 0,
            HighReliability = 1
        }

        public enum NETWORK_PROTOCOL
        {
            Ggp = 3,
            Icmp = 1,
            Idp = 22,
            Igmp = 2,
            IP = 4,
            ND = 77,
            Pup = 12,
            Tcp = 6,
            Udp = 17,
            Other = -1
        }

        private const int RAW_WIDTH_BYTES = 8 * 32;

        private const int MIN_PACKET_SIZE = 20;
        private const int MIN_HEADER_SIZE = 5;
        private const int SHIFT_4 = 4;
        private const int AND_VERSION = 240; //0xF0
        private const int AND_HEADER_LENGTH = 15; //0xF
        private const int AND_PRECEDENCE = 224;
        private const int AND_DELAY = 16; //0x10
        private const int AND_THROUGHPUT = 8;
        private const int AND_RELIABLITIY = 4;

        private const int RAW_CHECKSUM_INDEX_0 = 11;
        private const int RAW_CHECKSUM_INDEX_1 = 10;
        private const int RAW_PRTOCOL = 9;


        private const int SHIFT_PRECEDENCE = 5;
        private const int SHIFT_DELAY = 4;
        private const int SHIFT_THROUGHPUT = 3;
        private const int SHIFT_RELIABILITY = 2;


        public DateTime time { get; set; }
        public int version { get; set; }
        public int headerLength { get; set; }
        public TYPE_OF_SERVICE_PRECENDENCE precedence { get; set; }
        public TYPE_OF_SERVICE_DELAY delay { get; set; }
        public TYPE_OF_SERVICE_THROUGHPUT throughput { get; set; }
        public TYPE_OF_SERVICE_RELIABLITIY reliability { get; set; }
        public int totalLength { get; set; }
        public int identification { get; set; }
        public int timeToLive { get; set; }
        public NETWORK_PROTOCOL protocol { get; set; }
        public byte[] checksum { get; set; }
        public IPAddress sourceAddress { get; set; }
        public IPAddress destinationAddress { get; set; }
        public int sourcePort { get; set; }
        public int destinationPort { get; set; }

        public SnifferPacket(byte[] _raw)
        {
            parseRow(_raw);
        }

        private void parseRow(byte[] raw) {

            if (raw.Length < MIN_PACKET_SIZE) {
                throw new ArgumentException("Invalid Packet Size");
            }

            time = DateTime.Now;
            version = (raw[0] & AND_VERSION) >> SHIFT_4;

            headerLength = (raw[0] & AND_HEADER_LENGTH) * SHIFT_4;
            if(headerLength < MIN_HEADER_SIZE)
            {
                throw new ArgumentException("Invalid Header Size");
            }

            precedence = (TYPE_OF_SERVICE_PRECENDENCE)((raw[1] & AND_PRECEDENCE) >> SHIFT_PRECEDENCE);
            delay = (TYPE_OF_SERVICE_DELAY)((raw[1] & AND_DELAY) >> SHIFT_DELAY);
            throughput = (TYPE_OF_SERVICE_THROUGHPUT)((raw[1] & AND_THROUGHPUT) >> SHIFT_THROUGHPUT);
            reliability = (TYPE_OF_SERVICE_RELIABLITIY)((raw[1] & AND_RELIABLITIY) >> SHIFT_RELIABILITY);
            totalLength = raw[2] * RAW_WIDTH_BYTES + raw[3];

            if (totalLength != raw.Length) {
                throw new ArgumentException("Invalid Packet Length!");
            }

            identification = raw[4] * RAW_WIDTH_BYTES + raw[5];
            timeToLive = raw[8];

            if (Enum.IsDefined(typeof(NETWORK_PROTOCOL), (int)raw[9]))
            {
                protocol = (NETWORK_PROTOCOL)raw[RAW_PRTOCOL];
            }
            else
            {
                protocol = NETWORK_PROTOCOL.Other;
            }

            checksum = new byte[2];
            checksum[0] = raw[RAW_CHECKSUM_INDEX_0];
            checksum[1] = raw[RAW_CHECKSUM_INDEX_1];

            sourceAddress = new IPAddress(BitConverter.ToUInt32(raw, 12));
            destinationAddress = new IPAddress(BitConverter.ToUInt32(raw, 16));

            if (protocol == NETWORK_PROTOCOL.Tcp || protocol == NETWORK_PROTOCOL.Udp)
            {
                sourcePort = raw[headerLength] * RAW_WIDTH_BYTES + raw[headerLength + 1];
                destinationPort = raw[headerLength + 2] * RAW_WIDTH_BYTES + raw[headerLength + 3];
            }
            else
            {
                sourcePort = -1;
                destinationPort = -1;
            }

        }

       
    }
}
