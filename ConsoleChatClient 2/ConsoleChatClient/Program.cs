using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatProtocol;


namespace ConsoleChatClient
{
    class Program
    {
        private static TcpClient client;
        private static IPEndPoint IPEnd;

        private static string ip = "127.0.0.1";
        private static int port = 8081;
        private static string message = "";
        private static string name = "";

        static void Main(string[] args)
        {
            client = new TcpClient();
            IPEnd = new IPEndPoint(IPAddress.Parse(ip), port);
            client.Connect(IPEnd);

            Console.Write("Enter Name: ");
            name = Console.ReadLine();
            Transfer.SendTCP(client, new DataMessage() { message = name });

            ReciveAsync();

            while (!message.Equals("exit"))
            {
                Console.Write($"[{IPEnd.Address}] {name}: ");
                message = Console.ReadLine();

                Transfer.SendTCP(client, new DataMessage() { message = message });
            }

        }

        private static async void ReciveAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    string message = ((DataMessage)Transfer.ReceiveTCP(client)).message;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine(message);
                    Console.Write($"[{IPEnd.Address}] {name}: ");
                }
            });
        }
    }
}
