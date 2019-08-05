using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHHCorelib
{
    [Serializable]
    public class RES_Login : PACKET_HADER
    {
        public int UserIdx;
        public string UserName;
        public string SessionKey;
    }
}
