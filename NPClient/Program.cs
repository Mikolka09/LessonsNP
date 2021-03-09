using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace NPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEnd = new IPEndPoint(iP, 1337);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                socket.Connect(iPEnd);
                Thread.Sleep(2000);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
            }
        }
    }
}
