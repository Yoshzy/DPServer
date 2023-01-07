using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DPServer
{
    internal class Server
    {
        static IPEndPoint ipEnd_Server;
        static Socket socket_Server;
        public static string IPEndere = "127.0.0.1";
        public static int HostPort = 1202;
        public static string RecFolder = @"D:\xampp\htdocs\Fotos\";

        public static void ServerStart()
        {
            try
            {
                ipEnd_Server = new IPEndPoint(IPAddress.Parse(IPEndere), HostPort);
                socket_Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                socket_Server.Bind(ipEnd_Server);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            try
            {
                socket_Server.Listen(100000000);
                Console.WriteLine($"Servidor aguardando na porta {HostPort}.");

                Socket clienteSockt = socket_Server.Accept();
                clienteSockt.ReceiveBufferSize = 16384;
                byte[] clientData = new byte[1024 * 50000];
                int dataLength = clienteSockt.Receive(clientData, clientData.Length, 0);
                int fileNameLength = BitConverter.ToInt32(clientData, 0);
                string fileName = Encoding.UTF8.GetString(clientData, 4, fileNameLength);

                BinaryWriter bWriter = new BinaryWriter(File.Open(RecFolder + fileName, FileMode.Append));
                bWriter.Write(clientData, 4 + fileNameLength, dataLength - 4 - fileNameLength);

                while (dataLength > 0)
                {
                    dataLength = clienteSockt.Receive(clientData, clientData.Length, 0);
                    if (dataLength == 0)
                    {
                        bWriter.Close();
                    }
                    else
                    {
                        bWriter.Write(clientData, 0, dataLength);
                    }
                    Console.WriteLine($"Arquivo {fileName} salvo com sucesso!");
                    bWriter.Close();

                    clienteSockt.Close();
                    socket_Server.Close();
                    socket_Server.Dispose();
                    ServerStart();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

}
