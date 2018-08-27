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
            //create a new telnet connection to hostname "gobelijn" on port "23"
            TelnetConnection tc = new TelnetConnection("10.10.", 23);

            //login with user "root",password "rootpassword", using a timeout of 100ms, and show server output
            string s = tc.Login("root", "rootpassword", 100);
            Console.Write(s);

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

                // display server output
                Console.Write(tc.Read());
            }
            Console.WriteLine("***DISCONNECTED");
            Console.ReadLine();
        }
}
