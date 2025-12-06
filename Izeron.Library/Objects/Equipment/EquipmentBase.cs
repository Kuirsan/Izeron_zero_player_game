using Izeron.Library.Enums;
using Izeron.Library.Objects.LootableObjects;
using System;

namespace Izeron.Library.Objects.Equipment
{
    public abstract class EquipmentBase : LootableBaseObject
    {
        public EquipmentSlot Slot { get; private set; }
        public int AttackBonus { get; private set; }
        public int DefenseBonus { get; private set; }
        public string[] AllowedClasses { get; private set; }

        protected EquipmentBase(string name, int costValue, EquipmentSlot slot, int attackBonus, int defenseBonus, string[] allowedClasses = null) 
            : base(name, costValue)
        {
            Slot = slot;
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            AllowedClasses = allowedClasses;
        }

        public bool CanEquip(string className)
        {
            if (AllowedClasses == null || AllowedClasses.Length == 0) return true;
            foreach (var cls in AllowedClasses)
            {
                if (string.Equals(cls, className, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Loot()
        {
            // Logic for looting logic if needed
        }
    }
}
