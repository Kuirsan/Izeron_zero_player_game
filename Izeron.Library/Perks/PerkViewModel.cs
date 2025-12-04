using System;

namespace Izeron.Library.Perks
{
    public class PerkViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Bonus { get; set; }
        public int Level { get; set; }

        public PerkViewModel(ActivePerk perk)
        {
            Name = perk.Name;
            Description = perk.Description;
            Bonus = $"+{perk.Value} {GetPerkTypeRussian(perk.Type)}";
            Level = perk.AcquiredAtLevel;
        }

        private string GetPerkTypeRussian(string type)
        {
            return type.ToLower() switch
            {
                "health" => "Здоровье",
                "damage" => "Урон",
                "defense" => "Защита",
                _ => type
            };
        }
    }
}
