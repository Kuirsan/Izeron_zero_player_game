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

        public abstract HealthPotionBase GetHealthPotion();

        public abstract bool TryToAddItemToInventory(ILootable item);
        public abstract bool TryToRemoveFromInventory(ILootable item);

        public abstract bool TryToAddHealthPotion(HealthPotionBase potion);
        public abstract bool TryToRemoveHealthPotion(HealthPotionBase potion);

        public abstract bool SomethingInInventory(Func<string, bool> isQuestItem = null);

        public abstract bool HasHealthPotions();

        public abstract ILootable GetItemForSale(Func<string, bool> isQuestItem = null);
        public abstract List<LootViewModel> GetInventoryList();

        public abstract bool IsFullOfHealthPotions();

        public int NumberOfHealthPotion => _healthPotions.Count;
    }
}
