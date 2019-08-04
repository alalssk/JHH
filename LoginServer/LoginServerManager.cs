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
            byte[] buff = new byte[1024];

            while(true)
            {
                TcpClient tc = m_listener.AcceptTcpClient();
                
                tc.GetStream().ReadAsync(buff,0, buff.Length).ContinueWith(_ => 
                {
                    Console.WriteLine("Recive Client handle: {0}", tc.Client.Handle);
                    
                    var obj = JHHServerApi.Deserialize<PACKET_HADER>(buff);
                    if (EPacketType.REQ_Login == obj.type)
                    {
                        Request_Login packet = obj as Request_Login;
                        UserLogin userinfo = null;
                        switch (DBHelper.UserLoginChecker(packet.user_id, packet.user_pass, out userinfo))
                        {
                            case EAnswerType.Fail_Invailed_Password:
                                break;
                            case EAnswerType.Fail_NotFound_User:
                                break;
                            case EAnswerType.Success:

                                break;
                            default:
                                break;
                        }
                    }
                });
            }
        }
        
    }
}
