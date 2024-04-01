
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyApp
{
    internal class Program
    {

        //Got networking working by using https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services
        static async Task Main(string[] args)
        {
            while (true)
            {
                int key = 3;
                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("192.168.1.40");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint ipEndPoint = new(ipAddress, 11_000);
                Console.WriteLine(ipAddress);
                try
                {
                    //Ask the user if they want to send or recive 
                    Console.WriteLine("Are you Sending (0) or Reciving(1)");
                     key = Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Please only enter Numbers");
                }
                if (key == 0)
                {
                    using Socket client = new(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                    await client.ConnectAsync(ipEndPoint);
                    while (true)
                    {
                        // Send message.
                        var message = "Hi friends 👋!<|EOM|>";
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
                            break;
                        }
                      
                    }

                    client.Shutdown(SocketShutdown.Both);
                }
                else if (key == 1)
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

                            break;
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input");
                }

            }
        }
    }
}