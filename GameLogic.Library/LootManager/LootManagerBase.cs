using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LootManager
{
    public abstract class LootManagerBase
    {
        protected Queue<ILootable> _lootableObjects;
        public abstract void addLoot(ILootable loot);
        public abstract ILootable getNextLootableObject();
        public abstract void generateLootAndAddByFloor(int floor, int lootCount);

        public LootManagerBase()
        {
            _lootableObjects = new Queue<ILootable>();
        }
    }
}
