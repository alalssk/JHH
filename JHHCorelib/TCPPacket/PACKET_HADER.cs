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

    }
    public enum EAnswerType
    {
        None = -1,
        Fail_DBConnection,
        Success,
        Fail_Invailed_Password,
        Fail_NotFound_User,
    }
    public class PACKET_HADER
    {
        public EPacketType type;
    }
}
