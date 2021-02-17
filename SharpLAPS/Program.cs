using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLAPS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"
   _____ __                     __    ___    ____  _____
  / ___// /_  ____ __________  / /   /   |  / __ \/ ___/
  \__ \/ __ \/ __ `/ ___/ __ \/ /   / /| | / /_/ /\__ \ 
 ___/ / / / / /_/ / /  / /_/ / /___/ ___ |/ ____/___/ / 
/____/_/ /_/\__,_/_/  / .___/_____/_/  |_/_/    /____/  
                     /_/                             ");


            var parsed = ArgumentParser.Parse(args);
            String username = null;
            String password = null;
            String target = "*";
            String connectionString = "LDAP://{0}:{1}";
            DirectoryEntry ldapConnection;

            // Display help
            if (parsed.Arguments.ContainsKey("/help") || !parsed.Arguments.ContainsKey("/host"))
            {
                Console.WriteLine("Required");
                Console.WriteLine("/host:<1.1.1.1>  LDAP host to target, most likely the DC");

                Console.WriteLine("\nOptional");
                Console.WriteLine("/user:<username> Username of the account");
                Console.WriteLine("/pass:<password> Password of the account");
                Console.WriteLine("/target:<target> computer name (if not set query all computers in AD)");
                Console.WriteLine("/out:<file>      Outputting credentials to file");
                Console.WriteLine("/ssl             Enable SSL (LDAPS://)");

                Console.WriteLine("\nUsage: SharpLAPS.exe /user:DOMAIN\\User /pass:MyP@ssw0rd123! /host:192.168.1.1");
                Environment.Exit(-1);
            }

            // Handle LDAPS connection
            if (!parsed.Arguments.ContainsKey("/ssl"))
            {
                connectionString = String.Format(connectionString, parsed.Arguments["/host"], "389");
            }
            else
            {
                connectionString = String.Format(connectionString, parsed.Arguments["/host"], "636");
            }
            
            // Filter computer name
            if (parsed.Arguments.ContainsKey("/target"))
            {
                target = parsed.Arguments["/target"] + "$";
            }

            // Use the provided credentials or the current session
            if (parsed.Arguments.ContainsKey("/user") && parsed.Arguments.ContainsKey("/pass"))
            {
                Console.WriteLine("\n[+] Using the following credentials");
                Console.WriteLine("Host: " + connectionString);
                Console.WriteLine("User: " + parsed.Arguments["/user"]);
                Console.WriteLine("Pass: " + parsed.Arguments["/pass"]);
                username = parsed.Arguments["/user"];
                password = parsed.Arguments["/pass"];
            }
            else
            {
                Console.WriteLine("\n[+] Using the current session");
                Console.WriteLine("Host: " + connectionString);
            }

            try
            {
                // Connect to LDAP
                ldapConnection = new DirectoryEntry(connectionString, username, password, System.DirectoryServices.AuthenticationTypes.Secure);
                Console.WriteLine("\n[+] Extracting LAPS password from LDAP");
                DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
                searcher.Filter = "(&(objectCategory=computer)(ms-MCS-AdmPwd=*)(sAMAccountName=" + target + "))";

                // Iterate over all the credentials
                List<string> output = new List<string>();
                foreach (SearchResult result in searcher.FindAll())
                {
                    DirectoryEntry DirEntry = result.GetDirectoryEntry();
                    String sam = "Machine  : " + DirEntry.Properties["sAMAccountName"].Value;
                    String pwd = "Password : " + DirEntry.Properties["ms-Mcs-AdmPwd"].Value;
                    Console.WriteLine(sam);
                    Console.WriteLine(pwd);
                    output.Add(DirEntry.Properties["sAMAccountName"].Value + ":" + DirEntry.Properties["ms-Mcs-AdmPwd"].Value);

                }

                // Export the data to the provided file
                if (parsed.Arguments.ContainsKey("/out"))
                {
                    File.AppendAllLines(parsed.Arguments["/out"], output);
                }
            }
            catch
            {
                Console.WriteLine("\n[!] Invalid credentials or unreachable server");
            }
        }
    }
}
