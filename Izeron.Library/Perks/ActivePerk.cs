using System;

namespace Izeron.Library.Perks
{
    /// <summary>
    /// Активный перк персонажа
    /// </summary>
    public class ActivePerk
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public int AcquiredAtLevel { get; set; }
        public int Level { get; set; } = 1;
        public int BaseValue { get; set; }

        public ActivePerk(PerkModel model, int level)
        {
            Name = model.Name;
            Description = model.Description;
            Type = model.Type;
            Value = model.Value;
            BaseValue = model.Value;
            AcquiredAtLevel = level;
        }

        public void Stack()
        {
            Level++;
            Value += BaseValue;
        }

        public override string ToString()
        {
            return $"{Name} (Lvl {Level}) (+{Value} {Type})";
        }
    }
}
