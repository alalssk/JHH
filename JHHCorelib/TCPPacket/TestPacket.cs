using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHHCorelib
{
    [Serializable]
    public class TestPacket: PACKET_HADER
    {
        public string name { get; set; }
        public int num { get; set; }
        public string message { get; set; }
    }
}
