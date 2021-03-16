using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChat
{
    class Program
    {
        private static List<Client> clients;
        private static Socket server;
        private static IPEndPoint IPEnd;

        private static string ip = "127.0.0.1";
        private static int port = 8081;
        private static object lck;

        static void Main(string[] args)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEnd = new IPEndPoint(IPAddress.Parse(ip), port);
            clients = new List<Client>();
            lck = new object();
            server.Bind(IPEnd);
            server.Listen(50);

            Console.WriteLine($"Server started: {IPEnd.Address} : {IPEnd.Port}");


            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        Socket socket = server.Accept();
                        Task.Run(() =>
                        {
                            byte[] buff = new byte[4];
                            socket.Receive(buff);
                            int byteLength = BitConverter.ToInt32(buff, 0);
                            buff = new byte[byteLength];
                            socket.Receive(buff);
                            string name = Encoding.Unicode.GetString(buff);
                            Client client = new Client()
                            {
                                Name = name,
                                ClientSocket = socket,
                                ClientIPEnd = (IPEndPoint)socket.LocalEndPoint
                            };
                            clients.Add(client);
                            Console.WriteLine($"Client connected with Name: {client.Name} and IP: {client.ClientIPEnd.Address}");
                            SendToEveryone(client, $"{client.Name} joined to chat");

                            while (true)
                            {
                                buff = new byte[4];
                                client.ClientSocket.Receive(buff);
                                byteLength = BitConverter.ToInt32(buff, 0);
                                buff = new byte[byteLength];
                                client.ClientSocket.Receive(buff);
                                string message = "[" + client.ClientIPEnd.Address + "] "
                                                     + client.Name + ": "
                                                     + Encoding.Unicode.GetString(buff);
                                Console.WriteLine($"Message has been recived: {client.Name} and IP: {client.ClientIPEnd.Address}");
                                SendToEveryone(client, message);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });
            Console.ReadKey();
            Console.WriteLine("Server closing...");
            server.Close();
        }

        private static void SendToEveryone(Client client, string message)
        {
            lock (lck)
            {
                foreach (var c in clients)
                {
                    if (c != client)
                    {
                        byte[] buff = Encoding.Unicode.GetBytes(message);
                        c.ClientSocket.Send(BitConverter.GetBytes(buff.Length));
                        c.ClientSocket.Send(buff);
                    }
                }
                Console.WriteLine($"Message has been sent: {client.Name} and IP: {client.ClientIPEnd.Address}");
            }
        }
    }

    public class Client
    {
        public Socket ClientSocket { get; set; }
        public IPEndPoint ClientIPEnd { get; set; }
        public string Name { get; set; }
    }
}
