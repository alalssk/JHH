using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
namespace ServerCommon
{
    public class Log
    {
        public enum ELogType
        {
            Trace,
            Debug,
            Error,
            Fatal,
        }
        static Logger m_logger = null;

        static public void init(string _ServerName)
        {
            m_logger = LogManager.GetLogger(_ServerName);
        }
        static public void Write(ELogType _type, string _str)
        {
            switch(_type)
            {
                case ELogType.Trace:
                    m_logger.Trace(_str);
                    break;
                case ELogType.Debug:
                    m_logger.Debug(_str);
                    break;
                case ELogType.Error:
                    m_logger.Error(_str);
                    break;
                case ELogType.Fatal:
                    m_logger.Fatal(_str);
                    break;
            }
        }
        static public void Write(ELogType _type, string _operation, string _dataFormat, params object[] _dataParam)
        {
            Write(_type, string.Format("{0}:\"{1}\"", _operation, string.Format(_dataFormat, _dataParam)));
        }

        static public void Server(string message, [CallerMemberName]string operation = "")
        {
            Log.Write(ELogType.Debug, operation, message);
        }
    }
}
