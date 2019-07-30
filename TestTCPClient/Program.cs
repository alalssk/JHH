using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JHHCorelib;
using JHHCorelib.API;

namespace TestTCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TcpClient> liTcpClients = new List<TcpClient>();

            //for(int i=0;i<100;i++)
            //{
            //    liTcpClients.Add(new TcpClient("127.0.0.1", 7000));
            //}

            // (1) IP 주소와 포트를 지정하고 TCP 연결 
            TcpClient tc = new TcpClient("127.0.0.1", 7000);
            //TcpClient tc = new TcpClient("localhost", 7000);
            Console.WriteLine("connect to server socket[{0}]", tc.Client.Handle);
            Thread.Sleep(3000);
            string msg = "Hello World";
            byte[] buff = Encoding.ASCII.GetBytes(msg);

            // (2) NetworkStream을 얻어옴 
            NetworkStream stream = tc.GetStream();
            Console.WriteLine("start....press any key");
            ConsoleKeyInfo key = Console.ReadKey();
            BinaryFormatter fom = new BinaryFormatter();
            Stream GetStream = new MemoryStream();

            while (key.KeyChar != '1')
            {
                key = Console.ReadKey();
                // (3) 스트림에 바이트 데이타 전송
                stream.Write(buff, 0, buff.Length);

                // (4) 스트림으로부터 바이트 데이타 읽기
                byte[] outbuf = new byte[1024];
                //int nbytes = stream.Read(outbuf, 0, outbuf.Length);
                tc.Client.Receive(outbuf);

                var obj = JHHServerApi.Deserialize<TestPacket>(outbuf);

                Console.WriteLine("Name:{0}, num{1}, message{2}", obj.name, obj.num, obj.message);

                //string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);
                //Console.WriteLine($"{nbytes} bytes: {output}");
                Thread.Sleep(3000);

            }

            // (5) 스트림과 TcpClient 객체 닫기
            stream.Close();
            tc.Close();

        }
    }
}
