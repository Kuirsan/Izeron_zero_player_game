using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Objects.Potions
{
    public class SmallHealthPotion : HealthPotionBase
    {
        public override void HealPerson(IHealable healable)
        {
            healable.GetHeal(_healAmount);
        }
        public SmallHealthPotion() : base(nameof(SmallHealthPotion), 5, 30)
        {

        }
    }
}
