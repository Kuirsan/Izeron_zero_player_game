using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Objects.Potions
{
    public class smallHealthPotion : HealthPotionBase
    {
        public override void HealPerson(IHealable healable)
        {
            healable.getHeal(_healAmount);
        }
        public smallHealthPotion() : base(nameof(smallHealthPotion), 5, 30)
        {

        }
    }
}
