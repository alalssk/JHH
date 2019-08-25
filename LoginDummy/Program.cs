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
using ServerCommon;

namespace LoginDummy
{

    public class User:GlobalObject
    {
        public string UserName;
        public int UserIdx;

        public User(int _x, int _y)
        {
            m_ObjectType = ObjectType.User;
            name = "P";
            x = _x;
            y = _y;
        }

    }
    public class GlobalObject
    {
        public enum ObjectType
        {
            None = 0,
            User,
            Monster,
            Some,
        }
        public enum vector
        {
            None = 0,
            Left,
            Right,
            Up,
            Down,
            R_up,
            R_down,
            L_up,
            L_down
        }
        public ObjectType m_ObjectType;
        public string name;
        public int x, y;
        public GlobalObject() { }
        public GlobalObject(int _x, int _y)
        {
            name = ".";
            x = _x;
            y = _y;
        }
        public virtual void SetObject() { }
        //public void spawn(ObjectType _type, )
        public bool move(vector _v, ref GlobalObject[,] _map)
        {
            switch(_v)
            {
                case vector.Left:
                    x--;
                    break;
                case vector.Right:
                    x++;
                    break;
                case vector.Up:
                    y++;
                    break;
                case vector.Down:
                    y--;
                    break;
                case vector.R_up:
                    x++; y++;
                    break;
                case vector.R_down:
                    x++; y--;
                    break;
                case vector.L_up:
                    x--; y++;
                    break;
                case vector.L_down:
                    x--; y--;
                    break;
            }
            return true;
        }
    }
    public class Map
    {
        GlobalObject[,] m_location;
        //HashSet<GlobalObject>[,] m_loca;
        int MAX_x;
        int MAX_y;
        public Map(int _maxX, int _maxY)
        {
            MAX_x = _maxX;
            MAX_y = _maxY;
            m_location = new GlobalObject[MAX_x, MAX_y];

            //m_loca[0, 0] = new HashSet<GlobalObject>();
            for (int i = 0; i < MAX_x; i++)
            {
                for (int j = 0; j < MAX_y; j++)
                {
                    m_location[i, j] = new GlobalObject(i, j);
                }
            }
        }
        public void SetObject(GlobalObject _obj, int _x, int _y)
        {
            m_location[_x, _y] = _obj;
        }

    }

    public class DummyClient
    {
        bool AcceptRequestFlag = false;
        bool CreateSendFlag = false;

        Queue<KeyValuePair<TcpClient, byte[]>> m_liClient = new Queue<KeyValuePair<TcpClient, byte[]>>();
        int Count;
        object m_lock = new object();

        public void Init()
        {
            Log.init("Client-Local");
            Count = 0;
        }
        public void Run()
        {
            ThreadPool.QueueUserWorkItem(AcceptRequestThread);
            ThreadPool.QueueUserWorkItem(CreateUserSendTest);
            while(true)
            {
                Console.WriteLine("1. acceptThread stop or start");
                Console.WriteLine("2. SendThread stop or start");
                string key = Console.ReadLine();

                if (key == "1")
                {
                    AcceptRequestFlag = (AcceptRequestFlag == true) ? false : true;
                }
                else if(key =="2")
                {
                    CreateSendFlag = (CreateSendFlag == true) ? false : true;
                }
                else
                {
                    continue;
                }

            }

        }
        public void AcceptRequestThread(object _obj)
        {
            while(!AcceptRequestFlag)
            {
                TcpClient client = new TcpClient("127.0.0.1", 7000);
                byte[] sendbuff = new byte[1024];
                CreateSendBuff_Dummy(EPacketType.REQ_Login_CreateUser, $"Dummy_{Count++}", out sendbuff);

                KeyValuePair<TcpClient, byte[]> data =
                    new KeyValuePair<TcpClient, byte[]>(client, sendbuff);

                lock (m_lock)
                {
                    m_liClient.Enqueue(data);
                }
                Thread.Sleep(1);
            }
        }
        public void CreateUserSendTest(object _obj)
        {
            byte[] buff = new byte[1024];
            while (!CreateSendFlag)
            {
                if (!(m_liClient.Count > 0)) { Thread.Sleep(10); continue; }
                KeyValuePair<TcpClient, byte[]> info;

                lock (m_lock)
                {
                    info = m_liClient.Dequeue();
                }

                info.Key.Client.Send(info.Value);
                info.Key.GetStream().ReadAsync(buff, 0, buff.Length).ContinueWith(_ => 
                {
                    RES_Login res = JHHServerApi.Deserialize<RES_Login>(buff);
                    Log.Server($"Create User Compliete UserKey: {res.SessionKey}");
                });

            }
            
        }
        public void LoginTest()
        {

        }
        public bool CreateSendBuff_Dummy(EPacketType _sendType, string _str, out byte[] _outbuff)
        {
            _outbuff = null;
            REQ_Login req = new REQ_Login();
            bool ret = false;

            req.user_id = _str;

            req.user_pass = "1010229";

            req.PacketType = _sendType;
            
            _outbuff = JHHServerApi.Serialize<REQ_Login>(req);
            Log.Server($" Dummy Accept Ready ID: {_str} SendType: {_sendType}");
            return ret;
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            DummyClient client = new DummyClient();
            client.Init();
            client.Run();
        }
    }
}
