using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Objects.Potions
{
    public abstract class HealthPotionBase : AbstractMainObject,IBuyable
    {
        protected int _healAmount;
        protected int _cost;
        private protected HealthPotionBase(string Name,int HealAmount,int Cost) : base(Name)
        {
            _healAmount = HealAmount;
            _cost = Cost;
        }

        public int Cost => _cost;

        public virtual void Buy(AbstractPerson person)
        {
            if (CanBuy(person))
            {
                person.AddMoneyAmount(-Cost);
                person.AddHealthPotion(this);
            }
        }

        public virtual bool CanBuy(AbstractPerson person)
        {
            if (!person.IsAnyMoney()) return false;
            if (person.Money >= Cost)
            {
                return true;
            }
            return false;
        }

        public abstract void HealPerson(IHealable healable);
    }
}
