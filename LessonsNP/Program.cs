using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace LessonsNP
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEnd = new IPEndPoint(iP, 1337);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(iPEnd);
            socket.Listen(10);
            try
            {
                Socket socket1 = socket.Accept();
                Console.WriteLine("connect");
                socket1.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            Console.ReadKey();

        }
    }
}
