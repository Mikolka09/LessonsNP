using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChat
{
    class Program
    {
        private static Socket[] clients;
        private static Socket server;
        static void Main(string[] args)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp.);
            Task.Run(() =>
            {
                while (true)
                {

                }
            });
            Console.ReadKey();
            server.Close();
        }
    }
}
