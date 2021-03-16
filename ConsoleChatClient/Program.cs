using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleChatClient
{
    class Program
    {
        private static Socket client;
        private static IPEndPoint IPEnd;

        private static string ip = "127.0.0.1";
        private static int port = 8081;
        private static string message = "";
        private static string name = "";

        static void Main(string[] args)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEnd = new IPEndPoint(IPAddress.Parse(ip), port);
            client.Connect(IPEnd);

            Console.Write("Enter Name: ");
            name = Console.ReadLine();
            byte[] buff = Encoding.Unicode.GetBytes(name);
            client.Send(BitConverter.GetBytes(buff.Length));
            client.Send(buff);

           ReciveAsync();

            while (!message.Equals("exit"))
            {
                Console.Write($"[{IPEnd.Address}] {name}: ");
                message = Console.ReadLine();
               
                buff = Encoding.Unicode.GetBytes(message);
                client.Send(BitConverter.GetBytes(buff.Length));
                client.Send(buff);
            }

            
        }

        private static async void ReciveAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    byte[] buff1 = new byte[4];
                    client.Receive(buff1);
                    int byteLength = BitConverter.ToInt32(buff1, 0);
                    buff1 = new byte[byteLength];
                    client.Receive(buff1);
                    string message = Encoding.Unicode.GetString(buff1);
                    Console.WriteLine(message);
                }
            });
        }
    }
}
