﻿using System;
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
        /// <summary>
        /// This method runs the sending service of the program.
        /// </summary>
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