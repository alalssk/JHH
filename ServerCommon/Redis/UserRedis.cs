using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using TCPCore;

namespace ServerCommon
{
    public enum EFieldType : byte
    {
        None,
        Session,

    }
    public class UserRedis : SingleT<UserRedis>
    {
        //RedisHelper m_RedisHelper; 얘가 하는 작업을 정확하게 파악하자.
        ConnectionMultiplexer m_client;
        IDatabase m_db;
        //KEY
        string UserKey(int _userIdx) { return $"user:{_userIdx}"; }

        public void init()
        {
            m_client = ConnectionMultiplexer.Connect("localhost");
            m_db = m_client.GetDatabase();
        }
        //세션키값는 When.NotExist옵션을 넣고 하자.
        //Session Key 가 존재하면 중복로그인이라는 의미이지만,
        //유저가 팅겼을 경우를 생각해야한다.
        //Session Key 를 다시 만들어주고, 각 다른 서버에 존재하는 해당 유저데이터를 삭제한다.
        public bool SetUserSession(int _userIdx, string _key, When _eType = When.NotExists )
        {
            return m_db.HashSet(UserKey(_userIdx), EFieldType.Session.ToString(), _key, _eType);
        }
        public bool SetHashs(int _userIdx, EFieldType _type, string _value, When _eType = When.Always)
        {
            return m_db.HashSet(UserKey(_userIdx), _type.ToString(), _value, _eType);
        }
        public string GetHashs(int _userIdx, EFieldType _type)
        {
            var value = m_db.HashGet(UserKey(_userIdx), _type.ToString());
            if (true == value.IsNull)
                return null;

            return value;

        }

    }
}
