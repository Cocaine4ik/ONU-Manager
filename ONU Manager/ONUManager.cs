using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ONU_Manager
{
    class ONUManager
    {
        static void Main(string[] args)
        {   string login;
            string input; // client input
            string output; // server output
            string sn;
            int ponNumber = 0; // pon number
            int onuNumber = 0; // onu number
            int vlan = 1000; // default vlan

            StreamWriter fw;
            FileInfo checkfile = new FileInfo("check.txt");
            fw = checkfile.CreateText();
            fw.WriteLine("continue");

            // create a new telnet connection to hostname "10.10.110.115" on port "23"
            TelnetConnection tc = new TelnetConnection("10.10.110.115", 23);

            // login with user "admin", password "admin", using a timeout of 100ms, 
            // and show server output
            login = tc.Login("admin", "admin", 100);
            Console.Write(login);
            Console.Write(tc.Read());

            // show onu with out configure
            tc.WriteLine("show gpon onu uncfg");

            // make condition when we haven't got onu to configure
            output = tc.Read();
            if(output.Contains("No related information to show")) {

                Console.WriteLine("There are nothing to configure now!");
                fw.WriteLine("break");
                fw.Close();
                Console.ReadKey(true);
            }
            else  {
                Console.Write(output);
            
                // (157, 3) - pon number area
                string comparePon = output.Substring(157, 3);
                // (169, 12) - serial number area
                sn = output.Substring(169, 12);

                // checking substring comparePon 
                // some troubles here whene we register to 1 numeral pon!!!
                if (comparePon.IndexOf(":") == 1)
                    ponNumber = Convert.ToInt32(comparePon.Substring(0,1));
                        else ponNumber = Convert.ToInt32(comparePon.Substring(0,2));
            
                // remove the restriction on the output of text to the size of the window
                tc.WriteLine("terminal length 0");
                // show all onu in "ponNumber" pon
                tc.WriteLine("show gpon onu state gpon-olt_1/2/" + ponNumber);
                output = tc.Read();
                Console.Write(output);

                // search for "ONU Number:" substring
                int slotIndex = output.IndexOf("ONU Number:");

                String checkOnuNumber = output.Substring(slotIndex + 12, 3);
                if(checkOnuNumber.IndexOf("/") == -1) {
                    onuNumber =  Convert.ToInt32(output.Substring(slotIndex + 15, 3));
                Console.WriteLine(onuNumber);
                }
                else if (checkOnuNumber.IndexOf("/") == 2) {
                    onuNumber =  Convert.ToInt32(output.Substring(slotIndex + 15, 2));
                Console.WriteLine(onuNumber);
                }
                else {
                    onuNumber =  Convert.ToInt32(output.Substring(slotIndex + 15, 1));
                Console.WriteLine(onuNumber);
                }

                // make value next to last onu number value
                onuNumber += 1;

                // change vlan to standart
                vlan = 1000 + ponNumber;
            
                tc.WriteLine("configure terminal");
                Console. Write(tc.Read());

            tc.WriteLine("interface gpon-olt_1/2/" + ponNumber);
            tc.WriteLine("onu " + onuNumber + " type universal sn " + sn);
            Console. Write(tc.Read());          

            tc.WriteLine("onu " + onuNumber + " profile line 500m");
            Console. Write(tc.Read());

            tc.WriteLine("onu " + onuNumber + " profile remote standart");
            Console. Write(tc.Read());

            tc.WriteLine("exit");
            Console. Write(tc.Read());

            tc.WriteLine("interface gpon-onu_1/2/" + ponNumber + ":" + onuNumber);
            Console. Write(tc.Read());

            tc.WriteLine("switchport vlan " + vlan +" tag");
            Console. Write(tc.Read());
            
            tc.WriteLine("exit");
            Console. Write(tc.Read());

            tc.WriteLine("pon-onu-mng gpon-onu_1/2/" + ponNumber + ":" + onuNumber);
            Console. Write(tc.Read());

            tc.WriteLine("vlan port eth_0/1 mode tag vlan " + vlan);
            Console. Write(tc.Read());

            tc.WriteLine("show running-config interface gpon-onu_1/2/" + ponNumber + ":" + onuNumber);
            Console. Write(tc.Read());

            tc.WriteLine("exit");
            Console. Write(tc.Read());
            tc.WriteLine("exit");
            Console. Write(tc.Read());

            tc.WriteLine("show pon power onu-rx gpon-onu_1/2/" + ponNumber + ":" + onuNumber);
            Console. Write(tc.Read());

            }
            // exit from OLT console interface
           // Console.WriteLine("Press any key to exit.");
            //tc.WriteLine("exit");          

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

