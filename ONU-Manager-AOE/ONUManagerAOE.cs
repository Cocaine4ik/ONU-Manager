using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONU_Manager_AOE {
    class ONUManagerAOE {
        static void Main(string[] args) {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo("D:\\C# Projects\\ONU-Manager\\ONU-Manager\\bin\\Debug\\ONU Manager.exe");
            process.Start();
        }
    }
}
