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
        public DateTime reg_date { get; private set; }
        public DateTime last_login_date { get; private set; }

        public UserLogin() { }

        public UserLogin(string _userId, string _pass, DateTime _regDate)
        {
            platform_user_id = _userId;
            password = _pass;
            reg_date = _regDate;
            last_login_date = _regDate;
        }
        public string m_UserInsertQuery = "INSERT INTO login_user_info(platform_user_id, password, reg_date, last_login_date) " +
                                          "VALUES(@platform_user_id,@password,@reg_date,@last_login_date);" +
                                          "SELECT LAST_INSERT_ID();";
    }
}
