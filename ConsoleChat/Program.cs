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
            server.Bind(IPEnd);
            server.Listen(50);
            lck = new object();
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        server.AcceptAsync().ContinueWith((Task<Socket> t) =>
                        {
                            Socket socket = t.Result;
                            byte[] buff = new byte[10];
                            string name = Encoding.ASCII.GetString(buff);
                            Client client = new Client() 
                            { 
                                Name = name,
                                ClientSocket = socket,
                                ClientIPEnd = (IPEndPoint)socket.LocalEndPoint
                            };
                            clients.Add(client);
                            while (true)
                            {
                                buff = new byte[1024];
                                client.ClientSocket.Receive(buff);
                                string message = "[" + client.ClientIPEnd.Address + "] "
                                                     + client.Name + ": "
                                                     + Encoding.ASCII.GetString(buff);
                                lock (lck)
                                {
                                    foreach (var c in clients)
                                    {
                                        if (c != client)
                                        {
                                            c.ClientSocket.Send(Encoding.ASCII.GetBytes(message));
                                        }
                                    }
                                }
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
            server.Close();
        }
    }

    public class Client
    {
        public Socket ClientSocket { get; set; }
        public IPEndPoint ClientIPEnd { get; set; }
        public string Name { get; set; }
    }
}
