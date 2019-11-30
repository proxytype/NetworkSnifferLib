using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NetworkSnifferLib.Logic
{
    public class IpHandler
    {
        public List<IPAddress> IpList;
        public string ErrMsg { set; get; }

        private IPHostEntry HostEntry;
        private Regex _rx;

        public IPAddress IpSelected { get; set; }

        //Construtor: Encontra os Ip´s do Host, e insere na lista
        public IpHandler()
        {
            _rx = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
            try
            {
                this.IpList = new List<IPAddress>();
                this.ErrMsg = "";

                this.HostEntry = Dns.GetHostEntry((Dns.GetHostName()));
                if (HostEntry.AddressList.Length > 0)
                {
                    foreach (IPAddress ip in HostEntry.AddressList)
                    {
                        if (_rx.IsMatch(ip.ToString()))
                        {
                            this.IpList.Add(ip);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("IpHandler constructor exception: " + e.ToString());
            }

        }

        //Exibe todos os elementos(IP´s) inseridos na lista de endereços IP
        public void GetAllFoundIP()
        {
            int i = 0;
            foreach (IPAddress ip in IpList)
            {
                Console.WriteLine(IpList.IndexOf(ip)+") Ip: "+ip);
                i++;
            }

        }
        
        //Faz a verificação se o valor de opção(seleção de IP) corresponde às opções de seleção disponíveis, e seta o IP selecionado
        public bool SetIp(int ip)
        {

            if (ip < 0 || ip >= this.IpList.Count)
            {
                this.ErrMsg = "IP index out of range";
                return false;
            }
            
            if (!_rx.IsMatch(this.IpList[ip].ToString()))
            {
                this.ErrMsg = "IP v6 sniffing not supported";
                return false;
            }

            this.IpSelected = this.IpList[ip];
            return true;
        }

    }
}
