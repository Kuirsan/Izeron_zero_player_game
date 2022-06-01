using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LootManager
{
    public abstract class LootManagerBase
    {
        protected Queue<ILootable> _lootableObjects;
        public abstract void AddLoot(ILootable loot);
        public abstract ILootable GetNextLootableObject();
        public abstract void GenerateLootAndAddByFloor(int floor, int lootCount);

        public LootManagerBase()
        {
            _lootableObjects = new Queue<ILootable>();
        }
    }
}
