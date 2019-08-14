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

        //
        delegate RES_Login LoginProc(REQ_Login _req);
        Dictionary<EPacketType, LoginProc> m_LoginPro = new Dictionary<EPacketType, LoginProc>();

        public LoginServerManager()
        {
            //m_listener = new TcpListener(IPAddress.Any, 7000);
        }
        public void Init()
        {
            DBHelper.IsConnectCheck(EDBType.Login, add);
            UserRedis.SIG.init();
            m_LoginPro.Add(EPacketType.REQ_Login, UserLogin);
            m_LoginPro.Add(EPacketType.REQ_Login_CreateUser, CreateUser);

            m_loginDB = new LoginDB();
            m_listener = new TcpListener(IPAddress.Any, 7000);
            m_listener.Start();

            
        }
        #region[Login 델리게이트]
        public RES_Login UserLogin(REQ_Login _req)
        {
            RES_Login res = new RES_Login();
            UserLogin userinfo = null;
            res.AnswerType = DBHelper.UserLoginChecker(_req.user_id, _req.user_pass, out userinfo);
            switch (res.AnswerType)
            {
                case EAnswerType.Fail_Invailed_Password:
                    break;
                case EAnswerType.Fail_NotFound_User:
                    break;
                case EAnswerType.Success:
                    res.UserIdx = userinfo.user_idx;
                    res.UserName = userinfo.platform_user_id;
                    res.SessionKey = $"user:{userinfo.user_idx}-{DateTime.Now}";

                    if (false == UserRedis.SIG.SetUserSession(userinfo.user_idx, res.SessionKey))
                    {
                        UserRedis.SIG.SetUserSession(userinfo.user_idx, res.SessionKey, StackExchange.Redis.When.Always);
                        //S2S_UserInit(userinfo.user_idx); 다른서버에 유저초기화

                    }
                    //이거 보내고 소캣연결 끊기.
                    break;
                default:
                    break;
            }
            return res;
        }
        public RES_Login CreateUser(REQ_Login _req)
        {
            
            UserLogin CreateUserInfo = new UserLogin(_req.user_id, _req.user_pass, DateTime.Now);

            RES_Login res = new RES_Login();
            res.UserIdx = DBHelper.CreateUser(CreateUserInfo);
            res.UserName = _req.user_id;
            res.SessionKey = $"user:{res.UserIdx}-{DateTime.Now}";
            
            if(res.UserIdx > 0)
            {
                //여기서 세션키를 만들라면 Useridx가 필요한데
                //userIdx는 테이블에 AUTO INCREMENT 설정이 되어있기 떄문에. Insert 하면서 userIdx값을 받아오는 함수를 이용해야한다.
                //--> SELECT LAST_INSERT_ID(); 와  ExcuteSacla<int> 를 이용하자
                //어짜피 ORM 트랜잭션 시스템을 만들어야되서 대충만들어 놓기.

                UserRedis.SIG.SetUserSession(res.UserIdx, res.SessionKey);
                res.AnswerType = EAnswerType.Success;
            }
            else
            {
                res.AnswerType = EAnswerType.Fail_CreateUser;
            }

            return res;
        }
        #endregion
        public void Run()//얘를 main함수에서 ThreadPool로 실행시키면....?
        {
            //m_acceptThread = new Thread(Accept);
            ThreadPool.QueueUserWorkItem(Accept);
            ThreadPool.QueueUserWorkItem(Regulator);
            //m_acceptThread.Start();
            Console.WriteLine("Accpet Start...");

            bool isExit = false;
            while(!isExit)
            {
                Console.WriteLine("input key...");
                Console.WriteLine("1. view user");
                Console.WriteLine("2. Exit Server");

                string key = Console.ReadLine();
                if (key == "1")
                {
                    foreach (var info in m_DicClient)
                    {
                        Console.WriteLine("Key:{0}, ClientHandle:{1}", info.Key, info.Value.Client.Handle);
                    }
                    Console.WriteLine("UserCount: [{0}]", m_DicClient.Count);
                }
                else if (key == "2")
                {
                    isExit = true;
                }
                else continue;
            }
            //m_acceptThread.Join();

        }
        public void Accept(object _state)
        {
            while(true)
            {
                TcpClient tc = m_listener.AcceptTcpClient();
                AddUser(tc);
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
                Console.WriteLine("Recive Client handle: {0}, Thread_{1}", _client.Client.Handle, Thread.CurrentThread.ManagedThreadId);

                var obj = JHHServerApi.Deserialize<PACKET_HADER>(buff);
                REQ_Login req = obj as REQ_Login;
                RES_Login res = m_LoginPro[obj.PacketType](req);

                byte[] resBuff = JHHServerApi.Serialize<RES_Login>(res);
                Console.WriteLine("BUFF: {0}", resBuff);
                _client.Client.Send(resBuff);
                Console.WriteLine("응답패킷 전송 완료 ==> {0}, Thread_{1}", _client.Client.Handle, Thread.CurrentThread.ManagedThreadId);

                if (res.AnswerType != EAnswerType.Success)
                    SetReceiveAsync(_client);

                DeleteUser(_client);
                
            });
        }
        private void AddUser(TcpClient _tc)
        {
            while (true)
            {
                if (false == m_DicClient.TryAdd(_tc.Client.Handle, _tc))
                {
                    Thread.Sleep(10);
                    continue;
                }
                break;
            }
        }
        private void DeleteUser(TcpClient _tc)
        {
            TcpClient removeTc = null;
           
            while (true)
            {
                if (false == m_DicClient.TryRemove(_tc.Client.Handle, out removeTc))
                {
                    Thread.Sleep(10);
                    continue;
                }
                break;
            }
            Console.WriteLine("Delete complete");
        }
        /// <summary>
        /// m_DicClient를 관리하는 워커스레드.
        /// m_DicClient에서 연결이 끊긴 TC를 찾아 제거.
        /// 
        /// </summary>
        private void Regulator(object _state)
        {
            //_tc.Connected
            while(true)
            {
                if(m_DicClient.Count > 100)
                {
                    foreach (var info in m_DicClient)
                    {
                        if (false == info.Value.Connected)
                        {
                            DeleteUser(info.Value);
                        }
                    }
                }
                Thread.Sleep(5000);
                Console.Clear();
                Console.WriteLine("Regulator...Thread_{0}",Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("Total memory: {0:###,###,###,##0} bytes", GC.GetTotalMemory(true));
                Console.WriteLine("Private bytes {0}", System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64);
                Console.WriteLine("Handle count: {0}", System.Diagnostics.Process.GetCurrentProcess().HandleCount);
            }
        }
    }

}
