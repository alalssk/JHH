using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace LoginServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginServerManager server = new LoginServerManager();
            //멀로해야되는거지?
            //LoginServerManager.SIG.Init();
            server.Init();

            server.Run();

        }
    }
}
