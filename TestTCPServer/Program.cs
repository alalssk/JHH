using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using JHHCorelib;
using JHHCorelib.API;

namespace TestTCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Server m_server = new Server(3000,2048);
            //m_server.Init();
            //m_server.Start(new IPEndPoint(IPAddress.Any, 7000));

            // (1) 로컬 포트 7000 을 Listen
            TcpListener listener = new TcpListener(IPAddress.Any, 7000);
            listener.Start();
            Console.WriteLine("TCP Server Start");
            byte[] buff = new byte[1024];

            while (true)
            {
                // (2) TcpClient Connection 요청을 받아들여
                //     서버에서 새 TcpClient 객체를 생성하여 리턴
                //listener.AcceptTcpClientAsync();
                TcpClient tc = listener.AcceptTcpClient(); //Socket clientSock = tc.Client;

                Console.WriteLine("Connect client: {0} ", tc.Client.Handle);

                // (3) TcpClient 객체에서 NetworkStream을 얻어옴 
                NetworkStream stream = tc.GetStream();
                //BinaryFormatter fom = new BinaryFormatter();
                TestPacket t = new TestPacket();
                t.name = "21234";
                t.num = 123;

                //Stream byteStream = new MemoryStream();

                //fom.Serialize(byteStream, t);

                // (4) 클라이언트가 연결을 끊을 때까지 데이타 수신
                int nbytes;
                while ((nbytes = stream.Read(buff, 0, buff.Length)) > 0)
                {
                    // (5) 데이타 그대로 송신
                    //stream.Write(buff, 0, nbytes);
                    Console.WriteLine("Send to client: {0} ", tc.Client.Handle);
                    t.message = buff.ToString();
                    byte[] serialBuff = JHHServerApi.Serialize<TestPacket>(t);

                    tc.Client.Send(serialBuff, serialBuff.Length, 0);
                }

                // (6) 스트림과 TcpClient 객체 
                stream.Close();
                tc.Close();

                // (7) 계속 반복
            }
        }
    }
}
