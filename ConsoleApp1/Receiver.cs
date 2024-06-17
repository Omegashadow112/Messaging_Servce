using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messaging_Service
{
    /// <summary>
    /// This class runs the receiving service of the program.
    /// </summary>
    /// <param name="ipHostInfo">Adds the ip information of the user to the class</param>
    internal class Receiver
    {
        private IPAddress ipAddress;
        private IPEndPoint ipEndPoint;
        public Receiver(IPHostEntry ipHostInfo)
        { 
            ipAddress = ipHostInfo.AddressList[0];
            ipEndPoint = new(ipAddress, 11_000);
        }
        public void printf()
        {
            Console.WriteLine(ipAddress);
            Console.WriteLine(ipEndPoint);

        }
        
        /// <summary>
        /// This method runs the receiving service of the program.
        /// </summary>
        public  async void startService()
        {
            using Socket listener = new(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(100);

            var handler = await listener.AcceptAsync();
            while (true)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var eom = "<|EOM|>";
                if (response.IndexOf(eom) > -1 /* is end of message */)
                {
                    Console.WriteLine(
                        $"Socket server received message: \"{response.Replace(eom, "")}\"");

                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);
                    Console.WriteLine(
                        $"Socket server sent acknowledgment: \"{ackMessage}\"");

                }

            }
        }
    }
}
