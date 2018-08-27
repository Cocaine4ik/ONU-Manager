using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONU_Manager
{
    class ONUManager
    {
        static void Main(string[] args)
        {
            TelnetClient a = new TelnetClient();
            a.Connect("10.10.110.115", 23);
        }
    }
}
