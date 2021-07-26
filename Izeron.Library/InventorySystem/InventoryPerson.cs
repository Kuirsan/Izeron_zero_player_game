using Izeron.Library.Interfaces;
using Izeron.Library.Objects.LootableObjects;
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

        public override List<LootViewModel> GetInventoryList()
        {
            Dictionary<string, LootViewModel> keyValPairs = new Dictionary<string, LootViewModel>();
            foreach(var inv in _inventory)
            {
                if(inv is LootableBaseObject obj)
                {
                    if(keyValPairs.ContainsKey(obj.Name))
                    {
                        keyValPairs[obj.Name].Cost += obj.Volume;
                        keyValPairs[obj.Name].qty++;
                    }
                    else
                    {
                        keyValPairs.Add(obj.Name, new LootViewModel(obj));
                    }
                }
            }
            return keyValPairs.Values.ToList();
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
