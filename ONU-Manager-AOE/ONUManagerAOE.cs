using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ONU_Manager_AOE {
    class ONUManagerAOE {
        static void Main(string[] args) {

            String status;
            StreamReader fr = new StreamReader("D:\\C# Projects\\ONU-Manager\\ONU-Manager\\bin\\Debug\\check.txt");

            Process process = new Process(); // Create new process
            // locate ONU Manager.exe 
            process.StartInfo = new ProcessStartInfo("D:\\C# Projects\\ONU-Manager\\ONU-Manager\\bin\\Debug\\ONU Manager.exe");
            
            // test for fr
            if(fr.Equals("continue")) Console.WriteLine("ok");

            // create a flag cheacking if the process is active
            status = Process.GetProcessesByName("ONU Manager").Any() ? "active" : "inactive";

            // start ONU Manager.exe
            while (fr.Equals("continue") && status == "inactive")  {
                process.Start();
            }
        }
    }
}
