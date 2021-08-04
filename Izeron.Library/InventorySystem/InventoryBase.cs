using Izeron.Library.Interfaces;
using Izeron.Library.Objects.LootableObjects;
using Izeron.Library.Objects.Potions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.InventorySystem
{
    public abstract class InventoryBase
    {
        private protected int _capacity;

        private protected int _healthPotionCapacity = 3;

        private protected List<ILootable> _inventory;

        private protected List<HealthPotionBase> _healthPotions;

        protected abstract void Add(ILootable item);
        protected abstract void Remove(ILootable item);

        public abstract HealthPotionBase getHealthPotion();

        public abstract bool tryToAddItemToInventory(ILootable item);
        public abstract bool tryToRemoveFromInventory(ILootable item);

        public abstract bool tryToAddHealthPotion(HealthPotionBase potion);
        public abstract bool tryToRemoveHealthPotion(HealthPotionBase potion);

        public abstract bool somethingInInventory();

        public abstract bool hasHealthPotions();

        public abstract ILootable getItemForSale();
        public abstract List<LootViewModel> GetInventoryList();

        public abstract bool isFullOfHealthPotions();

        public int NumberOfHealthPotion => _healthPotions.Count;
    }
}
