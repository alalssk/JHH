using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JHHCorelib;

namespace JHHCorelib
{
    public class JhhException: Exception
    {
        EAnswerType AnswerType = EAnswerType.None;
        public JhhException(EAnswerType _type, string _str):base(_str)
        {
            AnswerType = _type;
        }
           
    }
}
