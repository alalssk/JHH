using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JHHCorelib
{
    [Serializable]
    public class REQ_Login : PACKET_HADER
    {
        public string user_id;
        public string user_pass;
    }

}