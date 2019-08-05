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
        public static bool CreateSendBuff(string _str, out byte[] _outbuff)
        {
            _outbuff = null;
            REQ_Login req = new REQ_Login();
            bool ret = false;

            Console.Write("* ID 입력  ===> *");
            req.user_id = Console.ReadLine();

            Console.Write("* PASSWORD 입력  ===> *");
            req.user_pass = Console.ReadLine();

            if (_str == "1")
            {
                req.PacketType = EPacketType.REQ_Login;
                ret = true;
            }
            else if (_str == "2")
            {
                Console.Write("PASSWORD 재입력 ===> ");
                if(req.user_pass == Console.ReadLine())
                {
                    Console.WriteLine("비번 다시 입력하셈");
                    return false;
                }
                req.PacketType = EPacketType.REQ_Login_CreateUser;
                ret = true;
            }
            else
            {
                Console.WriteLine("입력오류");
                return false;
            }
            _outbuff = JHHServerApi.Serialize<REQ_Login>(req);

            return ret;
        }
        static void Main(string[] args)
        {
            List<TcpClient> liTcpClients = new List<TcpClient>();

            for (int i = 0; i < 99; i++)
            {
                liTcpClients.Add(new TcpClient("127.0.0.1", 7000));
            }

            // (1) IP 주소와 포트를 지정하고 TCP 연결 
            TcpClient tc = new TcpClient("127.0.0.1", 7000);

            Console.WriteLine("connect to server socket[{0}]", tc.Client.Handle);
            Thread.Sleep(3000);

            byte[] buff = new byte[1024];// = Encoding.ASCII.GetBytes(msg);
            byte[] outbuf = new byte[1024];
            // (2) NetworkStream을 얻어옴 
            NetworkStream stream = tc.GetStream();

            BinaryFormatter fom = new BinaryFormatter();
            //Stream GetStream = new MemoryStream();
            string input_key = null;
            while(true)
            {
                Console.WriteLine("****** 키 입력 ******");
                Console.WriteLine("* [1]: 로그인 시도  *");
                Console.WriteLine("* [2]: 테스트패킷 전송*");

                input_key = Console.ReadLine();
                byte[] _outbuff = null;

                if (false == CreateSendBuff(input_key, out _outbuff))
                    continue;

                tc.Client.Send(_outbuff);
                Console.WriteLine("패킷전송 완료");
                // send 와 receive 사이에 위 함수들이 위치하도록 한다.
                //EX) 
                // sendBuff 만들기
                // send()
                // receive()
                // receive buff 처리부분
                //stream.Write(buff, 0, buff.Length);
                tc.Client.Receive(outbuf);
                Console.WriteLine("응답패킷 받음");
                RES_Login res = JHHServerApi.Deserialize<RES_Login>(outbuf);

                //이런것들도 묶어서 델리게이트로 처리.
                switch (res.AnswerType)
                {
                    case EAnswerType.Success:
                        Console.WriteLine("Login Successed");
                        Console.WriteLine("Hello {0} !!", res.UserName);
                        Console.WriteLine("Your Session Key is [{0}]", res.SessionKey);
                        break;
                    case EAnswerType.Fail_NotFound_User:
                    case EAnswerType.Fail_Invailed_Password:
                    default:
                        Console.WriteLine("로그인 실패 {0}",res.AnswerType);
                        break;
                }
            }

            //while (key.KeyChar != '1')
            //{
            //    key = Console.ReadKey();
            //    // (3) 스트림에 바이트 데이타 전송
            //    stream.Write(buff, 0, buff.Length);

            //    // (4) 스트림으로부터 바이트 데이타 읽기
            //    byte[] outbuf = new byte[1024];
            //    //int nbytes = stream.Read(outbuf, 0, outbuf.Length);
            //    tc.Client.Receive(outbuf);

            //    var obj = JHHServerApi.Deserialize<Response_Login>(outbuf);

            //    Console.WriteLine("AnswerType: {0}, UserIdx: {1}, UserName: {2}, Key: {3}", 
            //        obj.type, obj.UserIdx, obj.UserName, obj.SessionKey);

            //    //string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);
            //    //Console.WriteLine($"{nbytes} bytes: {output}");
            //    Thread.Sleep(3000);

            //}

            // (5) 스트림과 TcpClient 객체 닫기
            stream.Close();
            tc.Close();

        }
    }
}
