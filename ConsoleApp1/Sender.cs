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
    /// This class runs the sendning service of the program.
    /// </summary>
    /// <param name="ipHostInfo">Adds the ip information of the user to the class</param>
    internal class Sender
    {
        private IPAddress ipAddress;
        private IPEndPoint ipEndPoint;
        private string pubKey;
        public Sender(IPHostEntry ipHostInfo)
        { 
            ipAddress = ipHostInfo.AddressList[0];
            ipEndPoint = new(ipAddress, 11_000);
        }
        public void printf()
        {
            Console.WriteLine(ipAddress);
            Console.WriteLine(ipEndPoint);

        }

        private async void SendKey(string KeyToSend)
        {
            using Socket client = new(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

            await client.ConnectAsync(ipEndPoint);
            var messageBytes = Encoding.UTF8.GetBytes($"{KeyToSend}<|EOM|>");
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine($"Socket client sent message: \"{KeyToSend}\"");

            // Receive ack.
            var buffer = new byte[1_024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            if (response == "<|ACK|>")
            {
                Console.WriteLine(
                    $"Socket client received acknowledgment: \"{response}\"");
            }
        }
        private async void ReciveKey()
        {
            using Socket listener = new(
                   ipEndPoint.AddressFamily,
                   SocketType.Stream,
                   ProtocolType.Tcp);
            listener.Bind(ipEndPoint);
            listener.Listen(100);

            var handler = await listener.AcceptAsync();

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
        //wait until both jobs are done 
        /// <summary>
        /// This method runs the handshake service.
        /// </summary>
        /// 
        public void Handshake(string KeyToSend)
        {
            //Add Task for each type of handshake 
            Thread t1 = new Thread(() => SendKey(KeyToSend));
            Thread t2 = new Thread(() => ReciveKey());

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();


        }
        /// <summary>
        /// This method runs the sending service of the program.
        /// </summary>
        /// 
        public async Task startService()
        {
            using Socket client = new(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

            await client.ConnectAsync(ipEndPoint);

            while (true)
            {
                // Send message.
                string message;
                while (true)
                {
                    Console.WriteLine("Please Enter Message");
                    message = Console.ReadLine();
                    if(message != null)
                    {
                        message = $"{message}<|EOM|>";
                        break;
                    }
                    
                }
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                Console.WriteLine($"Socket client sent message: \"{message}\"");

                // Receive ack.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == "<|ACK|>")
                {
                    Console.WriteLine(
                        $"Socket client received acknowledgment: \"{response}\"");
                }


            }
    }
    }
}
