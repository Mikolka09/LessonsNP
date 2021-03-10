using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace LessonsNP
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
                socket.Bind(iPEnd);
                socket.Listen(100);
                Semaphore semaphore = new Semaphore(20,20);
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    socket.AcceptAsync().ContinueWith((Task<Socket> t)=> //асинхронный метод, работает без ожидания выхода клиента
                    { 
                        semaphore.WaitOne();
                        if(t is null)
                        {
                            throw new ArgumentNullException(nameof(t));
                        }
                        Socket socketClient = t.Result;
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        byte[] data = new byte[256];
                        do
                        {
                            bytes = socketClient.Receive(data);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (socketClient.Available > 0);
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                        string message = "Ваше сообщение доставлено";
                        data = Encoding.Unicode.GetBytes(message);
                        socketClient.Send(data);
                        socketClient.Disconnect(true);  //проверяет когда клиент закроется
                        socketClient.Shutdown(SocketShutdown.Both);
                        socketClient.Close();
                        semaphore.Release();
                    });
                    Thread.Sleep(10);
                    //Socket socketClient = socket.Accept();
                    //StringBuilder builder = new StringBuilder();
                    //int bytes = 0;
                    //byte[] data = new byte[256];
                    //do
                    //{
                    //    bytes = socketClient.Receive(data);
                    //    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    //} while (socketClient.Available > 0);
                    //Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    //string message = "Ваше сообщение доставлено";
                    //data = Encoding.Unicode.GetBytes(message);
                    //socketClient.Send(data);
                    ////socketClient.Disconnect(true);  //проверяет когда клиент закроется
                    //socketClient.Shutdown(SocketShutdown.Both);
                    //socketClient.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.ReadKey();

        }
    }
}
