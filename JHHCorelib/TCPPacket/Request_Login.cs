using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHHCorelib
{
    public class Request_Login : PACKET_HADER
    {
        public string user_id;
        public string user_pass;
    }
}
