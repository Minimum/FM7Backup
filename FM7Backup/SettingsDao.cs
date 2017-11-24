using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FM7Backup
{
    public static class SettingsDao
    {
        public static AppSettings LoadSettings(String path)
        {
            AppSettings settings = null;

            try
            {
                using (FileStream file = new FileStream(path, FileMode.Open))
                using (StreamReader reader = new StreamReader(file))
                {
                    settings = JsonConvert.DeserializeObject<AppSettings>(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return settings;
        }

        public static bool SaveSettings(String path, AppSettings settings)
        {
            bool success = false;

            try
            {
                using (FileStream file = new FileStream(path, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.Write(JsonConvert.SerializeObject(settings));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return success;
        }
    }
}
