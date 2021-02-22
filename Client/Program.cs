using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Client
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(" 1 : TCP");
            Console.WriteLine(" 2 : DEBUG MODE (TCP)");
            Console.WriteLine(" 3 : Test");


            string selection = Console.ReadLine();

            switch (selection)
            {
                case "1":
                    TCPClient();
                    break;
                case "2":
                    debug_Mode();
                    break;
                case "3":
                    sendBytes();
                    break;
                default:
                    Console.WriteLine("Please select a valid option");
                    break;
            }
        }

        private static void TCPClient()
        {
            TcpClient client = new TcpClient();
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            StreamWriter sw;

            int timeOut = 1000;

            try
            {

                client.Connect(ipAddress, 1303);
                client.SendTimeout = timeOut;
                client.ReceiveTimeout = timeOut;

                string u_input = "";

                sw = new StreamWriter(client.GetStream());
                try
                {
                    do
                    {
                        u_input = Console.ReadLine();
                        sw.WriteLine(u_input);
                        sw.Flush();

                    } while (u_input != "stop");

                    Main(u_input.Split());
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR : trouble streaming client ~~~ \n" + e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : trouble connecting client ~~~ \n" + e);
            }
        }


        private static string default_host = "localhost";
        private static int timeout = 5000;

        private static TcpClient TCP_client;
        private static DateTime startTimer;

        public static StreamWriter sw;
        public static StringBuilder sb;

        public static int counter = 0;

        Socket s = null;

        private static void debug_Mode()
        {
            TCP_client = new TcpClient();

            try
            {
                TCP_client.Connect(default_host, 1303);
                TCP_client.SendTimeout = timeout;
                TCP_client.ReceiveTimeout = timeout;
                sw = new StreamWriter(TCP_client.GetStream());

                try
                {
                    while (true)
                    {
                        //sb = new StringBuilder("-", 10);
                        for (int i = 0; i < 10; i++)
                        {
                            //startTimer = DateTime.Now;
                            //sw.WriteLine(counter + " " + startTimer);
                            //sw.Flush();
                            //Thread.Sleep(1500);
                            //sb.Append("-");
                        }
                        sw.WriteLine(counter);
                        sw.Flush();
                        Thread.Sleep(1500);
                        counter++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Cannot connect to host");
                    Console.WriteLine("/n" + e);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: connection dropped.");
                Console.WriteLine("/n" + e);
            }
        }

        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }


        private static void sendBytes()
        {
            try
            {
                string test = Console.ReadLine();

                byte[] buffer = Encoding.ASCII.GetBytes(test);
                byte[] bytesReceived = new byte[256];

                using (Socket s = ConnectSocket("127.0.0.1", 1303))
                {
                    if(s == null)
                    {
                        Console.WriteLine("Connection failed");

                        s.Send(buffer, buffer.Length, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: connection dropped.");
                Console.WriteLine("/n" + e);
            }
        }

    }
}
