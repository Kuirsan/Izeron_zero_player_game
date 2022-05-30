using Izeron.Library.Interfaces;
using Izeron.Library.Objects.LootableObjects;
using Izeron.Library.Objects.Potions;
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
            _healthPotions = new List<HealthPotionBase>();
        }

        public override HealthPotionBase GetHealthPotion()
        {
            if (_healthPotions.Count > 0) return _healthPotions.First();
            return null;
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
                        keyValPairs[obj.Name].Qty++;
                    }
                    else
                    {
                        keyValPairs.Add(obj.Name, new LootViewModel(obj));
                    }
                }
            }
            return keyValPairs.Values.ToList();
        }


        public override ILootable GetItemForSale()
        {
            if(SomethingInInventory())
            {
                return _inventory.First();
            }
            return null;
        }

        public override bool HasHealthPotions()
        {
            return _healthPotions.Count > 0;
        }

        public override bool SomethingInInventory()
        {
            return _inventory.Count > 0;
        }

        public override bool IsFullOfHealthPotions()
        {
            return _healthPotions.Count >= _healthPotionCapacity;
        }

        public override bool TryToAddHealthPotion(HealthPotionBase potion)
        {
            if (_healthPotions.Count >= _healthPotionCapacity) return false;
            _healthPotions.Add(potion);
            return true;
        }

        public override bool TryToAddItemToInventory(ILootable item)
        {
            if (_inventory.Count >= _capacity) return false;
            Add(item);
            return true;
        }

        public override bool TryToRemoveFromInventory(ILootable item)
        {
            Remove(item);
            return true;
        }

        public override bool TryToRemoveHealthPotion(HealthPotionBase potion)
        {
            return _healthPotions.Remove(potion);
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
