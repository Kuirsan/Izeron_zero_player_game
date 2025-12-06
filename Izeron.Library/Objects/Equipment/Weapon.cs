using Izeron.Library.Enums;

namespace Izeron.Library.Objects.Equipment
{
    public class Weapon : EquipmentBase
    {
        public Weapon(string name, int costValue, int minDamage, int maxDamage, string[] allowedClasses = null) 
            : base(name, costValue, EquipmentSlot.Weapon, maxDamage, 0, allowedClasses)
        {
            // Simplified weapon, using maxDamage as AttackBonus per previous logic design
        }
    }
}
