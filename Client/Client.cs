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
    class Client
    {
        private static string default_Host = "127.0.0.1";
        private static int default_Port = 1303;
        private static int timeOut = 5000;

        public static StreamWriter sw;
        public static StringBuilder sb;

        private static IPAddress broadcast = IPAddress.Parse(default_Host);

        static void Main(string[] args)
        {
            Console.WriteLine(" 1 : TCP");
            Console.WriteLine(" 2 : DEBUG MODE (TCP)");
            Console.WriteLine(" 3 : sendBytes");
            Console.WriteLine(" 4 : UDP");
            Console.WriteLine(" 5 : DEBUG MODE (UDP)");

            Console.Write("\n\nSelection : ");

            string selection = Console.ReadLine();

            switch (selection)
            {
                case "1":
                    TCPClient();
                    break;
                case "2":
                    debug_ModeTCP();
                    break;
                case "3":
                    ConnectSocket(default_Host, default_Port);
                    break;
                case "4":
                    UdpClient(args);
                    break;
                case "5":
                    udp_Debug();
                    break;
                default:
                    Console.WriteLine("Please select a valid option");
                    break;
            }
        }

        //TCP Client
        #region TCP CLIENT


        private static void TCPClient()
        {
            TcpClient client = new TcpClient();


            try
            {
                client.Connect(default_Host, default_Port);
                client.SendTimeout = timeOut;
                client.ReceiveTimeout = timeOut;

                string u_input = "";

                NetworkStream stream = client.GetStream();

                try
                {
                    do
                    {
                        u_input = Console.ReadLine();

                        byte[] data = System.Text.Encoding.ASCII.GetBytes(u_input);

                        stream.Write(data, 0, data.Length);
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

        //private static void TCPClient()
        //{
        //    TcpClient client = new TcpClient();
        //    IPAddress ipAddress = IPAddress.Parse(default_Host);

        //    StreamWriter sw;

        //    try
        //    {
        //        client.Connect(ipAddress, default_Port);
        //        client.SendTimeout = timeOut;
        //        client.ReceiveTimeout = timeOut;

        //        string u_input = "";

        //        sw = new StreamWriter(client.GetStream());
        //        try
        //        {
        //            do
        //            {
        //                u_input = Console.ReadLine();
        //                sw.WriteLine(u_input);
        //                sw.Flush();

        //            } while (u_input != "stop");

        //            Main(u_input.Split());
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("ERROR : trouble streaming client ~~~ \n" + e);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("ERROR : trouble connecting client ~~~ \n" + e);
        //    }
        //}

        private static TcpClient TCP_client;

        public static int counter = 1;

        private static void debug_ModeTCP()
        {
            TCP_client = new TcpClient();

            try
            {
                TCP_client.Connect(default_Host, default_Port);
                TCP_client.SendTimeout = timeOut;
                TCP_client.ReceiveTimeout = timeOut;
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

        #endregion
        //TCP Client End

        //Bytes
        #region BYTES

        private static void ConnectSocket(string server, int port)
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
                Socket tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(default_Host, default_Port);

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

            NetworkStream socketStream = new NetworkStream(s);

            byte[] buffer = Encoding.ASCII.GetBytes(Console.ReadLine());

            socketStream.Write(buffer, 0, buffer.Length);
            socketStream.Flush();

        }

        public static bool firstRun = true;

        private static void startByteConnection()
        {
            TcpClient client = new TcpClient();
            IPAddress ipAddress = IPAddress.Parse(default_Host);

            try
            {
                client.Connect(ipAddress, default_Port);
                client.SendTimeout = timeOut;
                client.ReceiveTimeout = timeOut;

                sendBytes(client);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : trouble connecting client ~~~ \n" + e);
            }
        }

        private static void sendBytes(TcpClient client)
        {

            StreamWriter sw;

            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(Console.ReadLine());

                sw = new StreamWriter(client.GetStream());
                try
                {
                    sw.WriteLine(buffer);
                    sw.Flush();

                    Thread.Sleep(1000);

                    sendBytes(client);
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

        #endregion
        //Bytes End

        //UDP Client
        #region UDP CLIENT


        private static UdpClient UDP_Client;

        private static void UdpClient(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(Console.ReadLine());
                IPEndPoint endPoint = new IPEndPoint(broadcast, default_Port);

                string asciiString = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                if (asciiString == "back")
                {
                    Main(buffer.ToString().Split());
                }

                s.SendTo(buffer, endPoint);

                UdpClient(buffer.ToString().Split());

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Cannot connect to endpoint");
                Console.WriteLine("/n" + e);
            }
        }

        private static void udp_Debug()
        {
            int counter = 1;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                while (true)
                {
                    sb = new StringBuilder("-", 10);
                    for (int i = 0; i < 10; i++)
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(counter.ToString());
                        IPEndPoint endPoint = new IPEndPoint(broadcast, default_Port);

                        socket.SendTo(buffer, endPoint);
                        sb.Append("-");
                        counter++;
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Cannot connect to endpoint");
                Console.WriteLine("/n" + e);
            }

        }

        #endregion
        //UDP Client End
    }
}
