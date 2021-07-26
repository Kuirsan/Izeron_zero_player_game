using Izeron.Library.Interfaces;
using Izeron.Library.Objects.LootableObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.InventorySystem
{
    public abstract class InventoryBase
    {
        private protected int _capacity;
        private protected List<ILootable> _inventory;
        protected abstract void Add(ILootable item);
        protected abstract void Remove(ILootable item);
        public abstract bool tryToAddItemToInventory(ILootable item);
        public abstract bool tryToRemoveFromInventory(ILootable item);
        public abstract bool somethingInInventory();
        public abstract ILootable getItemForSale();
        public abstract List<LootViewModel> GetInventoryList();
    }
}
