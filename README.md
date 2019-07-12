# NetworkSnifferLib
Simple Network Sniffer for detetcing packages from all local network interfaces c#.

**Must run under administrator privilege**

Usage:
```C#
//get machine local ip's
IPAddress[] hosts = Dns.Resolve(Dns.GetHostName()).AddressList;
//create sniffer for each ip
workers = new SnifferWorker[hosts.Length];

for (int i = 0; i < workers.Length; i++)
{
 //flag if export also the packet data
 workers[i] = new SnifferWorker(true);
 //sign to report event
 workers[i].NewPacket += Worker_NewPacket;
 workers[i].start(hosts[i]);
}

//dont kill the application
while (true) {
 Thread.Sleep(1000);
}
```
