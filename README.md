# NetworkSnifferLib
Introducing a straightforward Network Sniffer tool developed in C#. This lightweight application is designed to detect and analyze packets across all local network interfaces. Utilizing C# for simplicity and efficiency, this sniffer provides an easy-to-use solution for monitoring network activity on various interfaces. Enhance your network analysis capabilities with this user-friendly tool that allows you to effortlessly track and inspect data packets across your local network.

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
