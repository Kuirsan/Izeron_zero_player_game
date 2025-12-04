using System;

namespace Izeron.Library.Perks
{
    /// <summary>
    /// Модель перка для загрузки из JSON
    /// </summary>
    public class PerkModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "Health", "Damage", "Defense", etc.
        public int Value { get; set; }
        public int MinLevel { get; set; } // Минимальный уровень для получения перка
        public int MaxLevel { get; set; } // Максимальный уровень для получения перка (0 = без ограничений)
        public int Rarity { get; set; } // 1-5, где 5 - самый редкий
        public string IconPath { get; set; } // Путь к иконке (опционально)
    }
}
