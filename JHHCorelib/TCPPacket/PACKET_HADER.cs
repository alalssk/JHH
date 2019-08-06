using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHHCorelib
{
    public enum EPacketType
    {
        None = -1,
        REQ_Login,
        RES_Login,

        //*****특수패킷
        REQ_Login_CreateUser = 10000,

    }
    public enum EAnswerType
    {
        None = -1,
        Fail_DBConnection,
        Fail_InvailedPacket,
        Fail_AlreadyHasSessionInfo,
        Success,
        Fail_Invailed_Password,
        Fail_NotFound_User,

    }

    [Serializable]
    public class PACKET_HADER
    {
        public EPacketType PacketType;
        public EAnswerType AnswerType = EAnswerType.None;
    }
}
