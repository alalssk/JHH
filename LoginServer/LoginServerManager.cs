using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TCPCore;
using ServerCommon;
using JHHCorelib.API;
using JHHCorelib;
using ServerCommon.DB;
using System.Data;

namespace LoginServer
{
    class LoginServerManager: SingleT<LoginServerManager>// 각 메니저들을 글로벌로 합치자.
    {
        string add = "database=dapper_test;server=localhost;port=3306;user id=devil_user;password=devil12345";//임시
        LoginDB m_loginDB;//글로벌 오브잭트로 만들어야하나??
        TcpListener m_listener;
        ConcurrentDictionary<IntPtr, TcpClient> m_DicClient = new ConcurrentDictionary<IntPtr, TcpClient>();
        Thread m_acceptThread;

        public LoginServerManager()
        {
            //m_listener = new TcpListener(IPAddress.Any, 7000);
        }
        public void Init()
        {
            DBHelper.IsConnectCheck(EDBType.Login, add);
            m_loginDB = new LoginDB();
            m_listener = new TcpListener(IPAddress.Any, 7000);
            m_listener.Start();
        }
        public void Run()//얘를 main함수에서 ThreadPool로 실행시키면....?
        {
            m_acceptThread = new Thread(Accept);
            m_acceptThread.Start();
            Console.WriteLine("Accpet Start...");

            m_acceptThread.Join();

        }
        public void Accept()
        {
            while(true)
            {
                TcpClient tc = m_listener.AcceptTcpClient();
                //이런식으로 해도되는지...? 테스트해보자
                //클라 두개만 켜두고 테스트는 완료함.
                SetReceiveAsync(tc);

            }
        }
        private void SetReceiveAsync(TcpClient _client)
        {
            byte[] buff = new byte[1024];
            _client.GetStream().ReadAsync(buff, 0, buff.Length).ContinueWith(_ =>
            {
                Console.WriteLine("Recive Client handle: {0}", _client.Client.Handle);

                var obj = JHHServerApi.Deserialize<PACKET_HADER>(buff);
                RES_Login res = new RES_Login();
                if (EPacketType.REQ_Login == obj.PacketType)
                {
                    REQ_Login req = obj as REQ_Login;
                    UserLogin userinfo = null;
                    res.AnswerType = DBHelper.UserLoginChecker(req.user_id, req.user_pass, out userinfo);
                    switch (res.AnswerType)
                    {
                        case EAnswerType.Fail_Invailed_Password:
                            break;
                        case EAnswerType.Fail_NotFound_User:
                            break;
                        case EAnswerType.Success:
                            res.UserIdx = userinfo.user_idx;
                            res.UserName = userinfo.user_id;
                            res.SessionKey = "일단아무거나만들기";
                            //이거 보내고 세션끊기.
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    res.AnswerType = EAnswerType.Fail_InvailedPacket;
                }
                byte[] resBuff = JHHServerApi.Serialize<RES_Login>(res);
                Console.WriteLine("BUFF: {0}", resBuff);
                _client.Client.Send(resBuff);
                Console.WriteLine("응답패킷 전송 완료 ==> {0}", _client.Client.Handle);

                if (res.AnswerType != EAnswerType.Success)
                    SetReceiveAsync(_client);
                
            });
        }        
    }
}
