using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatProtocol;

namespace ConsoleChat
{
    class Program
    {
        private static List<Client> clients;
        private static TcpListener server;
        private static IPEndPoint IPEnd;

        private static string ip = "127.0.0.1";
        private static int port = 8081;
        private static object lck;

        static void Main(string[] args)
        {
            IPEnd = new IPEndPoint(IPAddress.Parse(ip), port);
            server = new TcpListener(IPEnd);
            clients = new List<Client>();
            lck = new object();
            server.Start(50);

            Console.WriteLine($"Server started: {IPEnd.Address} : {IPEnd.Port}");


            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        TcpClient socket = server.AcceptTcpClient();
                        Task.Run(() =>
                        {

                            string name = ((DataMessage)Transfer.ReceiveTCP(socket)).message;
                            Client client = new Client()
                            {
                                Name = name,
                                ClientSocket = socket,
                                ClientIPEnd = (IPEndPoint)socket.Client.LocalEndPoint
                            };
                            clients.Add(client);
                            Console.WriteLine($"Client connected with Name: {client.Name} and IP: {client.ClientIPEnd.Address}");
                            SendToEveryone(client, $"{client.Name} joined to chat");

                            while (true)
                            {
                                string message = "[" + client.ClientIPEnd.Address + "] "
                                                     + client.Name + ": "
                                                     + ((DataMessage)Transfer.ReceiveTCP(socket)).message;

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
            Console.WriteLine("Server stoping...");
            server.Stop();
        }

        private static void SendToEveryone(Client client, string message)
        {
            lock (lck)
            {
                foreach (var c in clients)
                {
                    if (c != client)
                    {
                        Transfer.SendTCP(c.ClientSocket, new DataMessage(){ message = message});
                    }
                }
                Console.WriteLine($"Message has been sent: {client.Name} and IP: {client.ClientIPEnd.Address}");
            }
        }
    }

    public class Client
    {
        public TcpClient ClientSocket { get; set; }
        public IPEndPoint ClientIPEnd { get; set; }
        public string Name { get; set; }
    }
}
