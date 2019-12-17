using Json.Net;
using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Text;

namespace backend.Services
{
    public class Serializer 
    {
        private string _resourcePath;
        private string _resourceContent;

        public Serializer()
        {
            _resourcePath = Path.Combine(Directory.GetCurrentDirectory(), "appconfig.json");
            FileStream stream = File.OpenRead(_resourcePath);
            int length = (int) stream.Length;

            byte[] buffer = new byte[length];
            var getResourceTask = stream.ReadAsync(buffer, 0, length);
            getResourceTask.Wait();

            if (getResourceTask.IsCompletedSuccessfully)
                _resourceContent = Encoding.Default.GetString(buffer);
            else {
                Console.WriteLine("Nem sikerült elindítani az alkalmazást. Kérlek lépj kapcsolatba a fejlesztővel.");
                Environment.Exit(-1);
            }
        }

        // Szerver oldali események naplózása.
        //
        public string GetServerLogMessage(string key)
        {
            string result = null;

            JObject jsonObject = JObject.Parse(_resourceContent);
            result = (string) jsonObject["Application"]["ServerLog"][key];

            return result;
        }

        // Alapértelmezett beállítások lekérése.
        //
        public string GetDefaults(string key)
        {
            string result = null;

            JObject jsonObject = JObject.Parse(_resourceContent);
            result = (string) jsonObject["Application"]["Defaults"][key];

            return result;
        }

        // Email küldő szolgáltatás konfiguráció.
        //
        public string GetEmailServiceConfiguration(string key)
        {
            string result = null;

            JObject jsonObject = JObject.Parse(_resourceContent);
            result = (string) jsonObject["Email"][key];

            return result;
        }

        // Email tartalom lekérése.
        //
        public string GetEmail(string category, string key, params string[] substitutions)
        {
            string result = null;

            JObject jsonObject = JObject.Parse(_resourceContent);
            result = (string) jsonObject["Email"]["Templates"][category][key];

            for (int i = 0; i < substitutions.Length; ++i) {
                string pattern = "{" + i.ToString() + "}";
                result = result.Replace(pattern, substitutions[i]);
            }

            return result;
        }

        // Alkalmazás adatok lekérése.
        //
        public string GetApplication(string key)
        {
            string result = null;

            JObject jsonObject = JObject.Parse(_resourceContent);
            result = (string) jsonObject["Application"][key];

            return result;
        }
    }
}