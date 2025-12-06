using Izeron.Library.Enums;

namespace Izeron.Library.Objects.Equipment
{
    public class Armor : EquipmentBase
    {
        public Armor(string name, int costValue, EquipmentSlot slot, int defense, string[] allowedClasses = null) 
            : base(name, costValue, slot, 0, defense, allowedClasses)
        {
        }
    }
}
