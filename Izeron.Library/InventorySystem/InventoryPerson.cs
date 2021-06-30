using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Izeron.Library.InventorySystem
{
    public class InventoryPerson : InventoryBase
    {
        public InventoryPerson(int capacity)
        {
            _capacity = capacity;
            _inventory = new List<ILootable>();
        }

        public override ILootable getItemForSale()
        {
            if(somethingInInventory())
            {
                return _inventory.First();
            }
            return null;
        }

        public override bool somethingInInventory()
        {
            return _inventory.Count > 0;
        }

        public override bool tryToAddItemToInventory(ILootable item)
        {
            if (_inventory.Count >= _capacity) return false;
            Add(item);
            return true;
        }

        public override bool tryToRemoveFromInventory(ILootable item)
        {
            Remove(item);
            return true;
        }

        protected override void Add(ILootable item)
        {
            _inventory.Add(item);
            item.Loot();
        }

        protected override void Remove(ILootable item)
        {
            _inventory.Remove(item);
        }

    }
}
