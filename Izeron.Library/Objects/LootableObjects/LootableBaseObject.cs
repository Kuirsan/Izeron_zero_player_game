using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Objects.LootableObjects
{
    public abstract class LootableBaseObject : AbstractMainObject, ILootable
    {
        protected int _costValue;
        private protected LootableBaseObject(string Name,int costValue) : base(Name)
        {
            _costValue = costValue;
        }

        public int Volume => _costValue;

        public new string Name => _name;

        public abstract void Loot();

    }
}
