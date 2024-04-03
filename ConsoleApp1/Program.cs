
using System.Net;


namespace Messaging_Service
{
    internal class Program
    {

       
        //Got networking working by using https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services
        
        static async Task Main(string[] args)
        {
            Receiver Server = new Receiver(await Dns.GetHostEntryAsync("192.168.1.40"));
            //Thread serverThread = new Thread(new ThreadStart(Server.startService));
            //serverThread.Start();
            //serverThread.Join();
            Server.startService().Wait();
        }
    }
}