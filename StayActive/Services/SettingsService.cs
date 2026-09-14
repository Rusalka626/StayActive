using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StayActive.Services
{
    public static class SettingsService
    {
        private static readonly string FolderPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StayActive");

        private static readonly string FilePath = Path.Combine(FolderPath, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new AppSettings();

                string json = File.ReadAllText(FilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                // Si el archivo está corrupto o hay error de lectura, regresamos valores por defecto
                return new AppSettings();
            }
        }
        public static void Save(AppSettings settings)
        {
            Directory.CreateDirectory(FolderPath);
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
