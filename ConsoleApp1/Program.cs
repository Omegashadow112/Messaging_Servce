
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Messaging_Service
{
    internal class Program
    {

       
        //Got networking working by using https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services
        
        /*Start up proceess
         *  start Lisening server wait for a connection
         *  ask user which server to connect 
         *  when server connects exchange public key
         *  when message recive say who from
         *  allow exit 
         */

        //Start with figuring out asymetric encyption. get it working locally 
        static async Task Main(string[] args)
        {
            
            IPHostEntry serverIP = null;
            IPHostEntry clientIP = null;
            while (true)
            {
                Console.WriteLine("What is the IP of this machine.");
                serverIP = await Dns.GetHostEntryAsync($"{Console.ReadLine()}");
                Console.WriteLine("What is the IP of the machine you want to send messages to.");
                clientIP = await Dns.GetHostEntryAsync($"{Console.ReadLine()}");
                if(serverIP != null && clientIP != null ) 
                {
                    break;
                }
            }
            var rsa = new RSACryptoServiceProvider();
            string publicKeyXML = rsa.ToXmlString(false);
            string privateKeyXML = rsa.ToXmlString(true);

            byte[] data = Encoding.UTF8.GetBytes("Hello world!");
            System.Console.WriteLine($"String Before enc:{Encoding.UTF8.GetString(data)}");
            byte[] encryptedData = rsa.Encrypt(data, false);

            System.Console.WriteLine($"String After enc:{Encoding.UTF8.GetString(encryptedData)}");
            byte[] decryptedData = rsa.Decrypt(encryptedData, false);
            string message = Encoding.UTF8.GetString(decryptedData);
            Console.WriteLine(message);

            Receiver Server = new Receiver(serverIP);
            //Thread serverThread = new Thread(new ThreadStart(Server.startService));
            //serverThread.Start();
            Sender Client = new Sender(clientIP);
            Client.Handshake(publicKeyXML);
            //Client.startService().Wait();
            //serverThread.Join();

        }
    }
}