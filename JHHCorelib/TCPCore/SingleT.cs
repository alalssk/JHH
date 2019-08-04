using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPCore
{
    public class SingleT<T> where T: class, new()
    {
        public static T SIG { get; private set; }
        static SingleT()
        {
            if(null == SingleT<T>.SIG)
            {
                SingleT<T>.SIG = new T();
            }
        }
    }
}
