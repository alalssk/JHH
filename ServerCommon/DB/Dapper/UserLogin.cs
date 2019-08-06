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
        public string platform_user_id { get; private set; }
        public string password { get; private set; }
        public DateTime last_login_date { get; private set; }


    }
}
