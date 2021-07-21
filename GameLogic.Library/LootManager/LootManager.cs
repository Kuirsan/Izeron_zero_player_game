using Izeron.Library.Interfaces;
using Izeron.Library.Objects.LootableObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LootManager
{
    public class LootManager : LootManagerBase
    {
        public override void addLoot(ILootable loot)
        {
            _lootableObjects.Enqueue(loot);
        }

        public override void generateLootAndAddByFloor(int floor, int lootCount)
        {
            for(int i = 0; i < lootCount; i++)
            {
                CommonLoot loot = new CommonLoot("Branch", new Random().Next(1, 1 + floor));
                addLoot(loot);
            }
        }

        public override ILootable getNextLootableObject()
        {
            if (_lootableObjects.Count == 0) return null;
            return _lootableObjects.Dequeue();
        }
        public LootManager() : base()
        {

        }
    }
}
