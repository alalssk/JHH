using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.DB
{
    public class UserLogin
    {
        public int user_idx { get; private set; }
        public string user_id { get; private set; }
        public string user_pass { get; private set; }
        public DateTime last_login_time { get; private set; }


    }
}
