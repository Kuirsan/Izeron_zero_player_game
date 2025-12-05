using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Izeron.Library.Persons
{
    public static class HeroRegistry
    {
        private static Dictionary<string, int> _heroNames = new Dictionary<string, int>();
        private static readonly string _registryFileName = "hero_registry.json";
        private static string _registryPath;

        static HeroRegistry()
        {
            // Проверяем сначала в подпапке HeroRegistry, затем в текущей директории
            string heroRegistryDir = Path.Combine(Directory.GetCurrentDirectory(), "HeroRegistry");
            string possiblePath = Path.Combine(heroRegistryDir, _registryFileName);
            
            if (File.Exists(possiblePath))
            {
                _registryPath = possiblePath;
            }
            else
            {
                // Если файл не найден в подпапке, используем текущую директорию
                _registryPath = Path.Combine(Directory.GetCurrentDirectory(), _registryFileName);
            }
            
            LoadRegistry();
        }

        public static void LoadRegistry()
        {
            if (File.Exists(_registryPath))
            {
                try
                {
                    string json = File.ReadAllText(_registryPath);
                    _heroNames = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
                }
                catch
                {
                    _heroNames = new Dictionary<string, int>();
                }
            }
        }

        public static void SaveRegistry()
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                };
                string json = JsonSerializer.Serialize(_heroNames, options);
                File.WriteAllText(_registryPath, json);
            }
            catch
            {
                // Игнорируем ошибки сохранения
            }
        }

        public static string RegisterHero(string baseName)
        {
            if (_heroNames.ContainsKey(baseName))
            {
                _heroNames[baseName]++;
                SaveRegistry();
                return $"{baseName} {ToRoman(_heroNames[baseName])}";
            }
            else
            {
                _heroNames[baseName] = 1;
                SaveRegistry();
                return baseName;
            }
        }

        private static string ToRoman(int number)
        {
            if (number == 1) return ""; // Первый герой без номера
            
            string[] thousands = { "", "M", "MM", "MMM" };
            string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
            string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

            return thousands[number / 1000] +
                   hundreds[(number % 1000) / 100] +
                   tens[(number % 100) / 10] +
                   ones[number % 10];
        }

        public static Dictionary<string, int> GetAllHeroes()
        {
            return new Dictionary<string, int>(_heroNames);
        }
    }
}
