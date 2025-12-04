using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Izeron.Library.Perks
{
    /// <summary>
    /// Менеджер перков - загружает перки из JSON и управляет их выдачей
    /// </summary>
    public class PerkManager
    {
        private List<PerkModel> _availablePerks;
        private Random _random;

        public PerkManager()
        {
            _random = new Random();
            LoadPerks();
        }

        private void LoadPerks()
        {
            string fileName = @"PerksLibrary\perks.json";
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                _availablePerks = JsonSerializer.Deserialize<List<PerkModel>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                _availablePerks = new List<PerkModel>();
            }
        }

        /// <summary>
        /// Получить случайный перк для указанного уровня
        /// </summary>
        public ActivePerk GetRandomPerkForLevel(int level)
        {
            // Фильтруем перки по уровню
            var eligiblePerks = _availablePerks.Where(p => 
                p.MinLevel <= level && 
                (p.MaxLevel == 0 || p.MaxLevel >= level)
            ).ToList();

            if (eligiblePerks.Count == 0)
            {
                return null;
            }

            // Создаем взвешенный список на основе редкости (чем выше редкость, тем ниже шанс)
            var weightedPerks = new List<PerkModel>();
            foreach (var perk in eligiblePerks)
            {
                int weight = Math.Max(1, 6 - perk.Rarity); // Редкость 5 = вес 1, редкость 1 = вес 5
                for (int i = 0; i < weight; i++)
                {
                    weightedPerks.Add(perk);
                }
            }

            // Выбираем случайный перк из взвешенного списка
            var selectedPerk = weightedPerks[_random.Next(weightedPerks.Count)];
            return new ActivePerk(selectedPerk, level);
        }

        /// <summary>
        /// Получить все доступные перки
        /// </summary>
        public List<PerkModel> GetAllPerks()
        {
            return new List<PerkModel>(_availablePerks);
        }

        /// <summary>
        /// Перезагрузить перки из файла
        /// </summary>
        public void ReloadPerks()
        {
            LoadPerks();
        }
    }
}
