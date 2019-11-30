using System;

using NetworkSnifferLib.Logic;

namespace NetworkSnifferLib.UI
{
    public class IpUI
    {
        public IpHandler IP;
        public SnifferWorker Sniff;

        //Construtor inicializa as variáveis das classes IpHandler e SnifferWorker
        public IpUI() 
        {
            IP = new IpHandler();
            Sniff = new SnifferWorker(true);
        }

        //Lista todos ip´s encontrados no Host, seleciona o IP, e chama a função de obtenção de pacotes
        public bool RenderMenu()
        {
            Console.WriteLine("---------- Choose your IP --------------");
            IP.GetAllFoundIP(); 
            Console.WriteLine("------------------------");
            if (GetOption())
            {
                Start();
                return true;
            }

            if (IP.ErrMsg != "")
            {
                Console.WriteLine(IP.ErrMsg);
            }

            return false;
        }

        //Chama a função de obtenção de pacotes com o ip selecionado
        public void Start()
        {
            Console.WriteLine("---------- START --------------");
            
            Sniff.Start(this.IP.IpSelected);
            
        }

        //Lê e retorna o valor lido. A opção de valor selecionada corresponde à posição do IP na lista de IP´s
        public bool GetOption()
        {
            try
            {
                return this.IP.SetIp(Convert.ToInt32(Console.ReadLine())); 
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                this.Sniff.Destroy();
                return false;
            }

        }

        //Informa na tela os dados e informações obtidas durante a análise dos pacotes
        public void GetLogSniff()
        {
            Console.WriteLine(Sniff.TypeIp);

            try
            {
                foreach (var packet in Sniff.PacketLog)
                {
                    Console.WriteLine(packet);
                }
            }
            catch (Exception)
            {
                this.Sniff.Destroy();
            }
            finally
            {
                Console.WriteLine("Total Packages: " + Sniff.TotalPackages);
                Console.WriteLine("Total TCP Packages: " + Sniff.TotalTCPPackages);
                Console.WriteLine("Total UDP Packages: " + Sniff.TotalUDPPackages);
                Console.WriteLine("Total Lost Packages: " + (Sniff.TotalPackages - (Sniff.TotalTCPPackages + Sniff.TotalUDPPackages)));
            }
        }
    }
}
