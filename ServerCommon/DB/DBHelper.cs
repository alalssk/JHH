using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using ServerCommon.DB;
using JHHCorelib;

namespace ServerCommon
{
    public enum EDBType
    {
        None = -1,
        Login,
        Game,
        Template,
        Redis,
    }
    public class DBHelper
    {
        public static bool IsConnectCheck(EDBType _eType, string _connectionString)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    if (false == connection.Ping()) //IDbConnection db = new MySqlConnection 는 .Ping()이 없네...?
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    //Log.ErrorLog(ex.ToString());
                    //Log.ErrorLog("DB Connect Checker fail. Type:{0}. Msg:{1}", _eType.ToString(), ex.ToString());
                    return false;
                }
            }

            //Log.InfoLog("DB Connect Checker Success.Type:{0}. connString:{1}", _eType.ToString(), _connectionString);
            return true;
        }
        public static EAnswerType UserLoginChecker(string _id, string _pass, out UserLogin _refUserLoginInfo)
        {
            _refUserLoginInfo = null;
            using (IDbConnection db = new MySqlConnection("database=dapper_test;port=3306;user id=devil_user;password=devil12345;server=localhost;"))
            {
                try
                {
                    db.Open();
                    var result = db.Query<UserLogin>($"SELECT * from login_user_info where platform_user_id = '{_id}';");

                    if (0 == result.Count()) return EAnswerType.Fail_NotFound_User;

                    UserLogin userinfo = result as UserLogin;

                    if (userinfo.user_pass != _pass) return EAnswerType.Fail_Invailed_Password;

                    _refUserLoginInfo = userinfo;
                    return EAnswerType.Success;
                }
                catch (Exception e)
                {
                    return EAnswerType.Fail_DBConnection;
                }
            }
        }
    }
}
