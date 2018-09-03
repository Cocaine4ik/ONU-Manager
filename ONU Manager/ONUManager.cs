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
            string input = ""; // client input
            string output = ""; // server output
            int ponNumber = 0; // pon number
            int onuNumber = 0; // onu number
            int vlan = 1000;

            //create a new telnet connection to hostname "10.10.110.115" on port "23"
            TelnetConnection tc = new TelnetConnection("10.10.110.115", 23);

            //login with user "admin", password "admin", using a timeout of 100ms, and show server output
            string s = tc.Login("admin", "admin", 100);
            Console.Write(s);

            // read OLT answer after log in
            Console.Write(tc.Read());

            // show unconfig onu
            input = "show gpon onu uncfg";
            tc.WriteLine(input);

            output = tc.Read();
            if(output.Contains("No related information to show"))
            Console.WriteLine("There are nothing to configure now!");
            else Console.Write(output);

            // Must work correclt only if we have one or more onu to register
            
            // (157, 3) - pon number area
            string comparePon = output.Substring(157, 3);
            // (169, 12) - serial number area
            string sn = output.Substring(169, 12);

            // write default vlan
            vlan = 1000 + ponNumber;

            // check substring comparePon 
            // Console.WriteLine(comparePon);
            // Console.WriteLine(sn);
            if (comparePon.IndexOf(":") == 1)
                ponNumber = Convert.ToInt32(comparePon.Substring(0,1));
                    else ponNumber = Convert.ToInt32(comparePon.Substring(0,2));
            
            // Console.WriteLine(ponNumber);
            tc.WriteLine("terminal length 0");
            tc.WriteLine("show gpon onu state gpon-olt_1/2/" + ponNumber);
            output = tc.Read();
            Console.Write(output);

            
            // exit from OLT console interface
            Console.WriteLine("Press any key to exit.");
            tc.WriteLine("exit");
            Console.ReadKey(true);
            

            /*
            // server output should end with "$" or ">", otherwise the connection failed
            string prompt = s.TrimEnd();
            prompt = s.Substring(prompt.Length - 1, 1);
            if (prompt != "$" && prompt != ">")
                throw new Exception("Connection failed");

            prompt = "";

            // while connected
            while (tc.IsConnected && prompt.Trim() != "exit")
            {
                // display server output
                Console.Write(tc.Read());

                // send client input to server
                prompt = Console.ReadLine();
                tc.WriteLine(prompt);

            }
            Console.WriteLine("***DISCONNECTED");
            Console.ReadLine();
            */
        }
    }
}

