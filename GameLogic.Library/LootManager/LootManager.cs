using GameLogic.Library.LogicModels;
using Izeron.Library.Interfaces;
using Izeron.Library.Objects.LootableObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GameLogic.Library.LootManager
{
    public class LootManager : LootManagerBase
    {
        private List<GameLootModel> _lootsModels;
        public override void addLoot(ILootable loot)
        {
            _lootableObjects.Enqueue(loot);
        }

        public override void generateLootAndAddByFloor(int floor, int lootCount)
        {
            for (int i = 0; i < lootCount; i++)
            {
                LootableBaseObject loot = generateRandomLoot(floor);
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
            loadEnemiesModels();
        }

        private void loadEnemiesModels()
        {
            string fileName = @"LootsLibrary\loots.json";
            _lootsModels = getLootsModelsFromJSON(fileName);
        }

        private List<GameLootModel> getLootsModelsFromJSON(string path)
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<GameLootModel>>(jsonString);
        }

        private LootableBaseObject generateLoot(GameLootModel lootModel)
        {
            LootFillModel lootFillModel = new LootFillModel
            {
                Cost = new Random().Next(lootModel.CostInGoldRange.minParameter, lootModel.CostInGoldRange.maxParameter),
                Name = lootModel.Name
            };

            return new CommonLoot(lootFillModel);
        }

        public LootableBaseObject generateRandomLoot(int floor)
        {
            LootableBaseObject loot = null;
            if (_lootsModels == null) return null;
            List<GameLootModel> lootModels = _lootsModels
                                                    .Where(x => x.FloorRange.minParameter <= floor && x.FloorRange.maxParameter >= floor)
                                                    .Select(x => x).ToList();
            if (lootModels.Count == 0) return null;
            if (lootModels.Count == 1) loot = generateLoot(lootModels.First());
            if (lootModels.Count > 1)
            {
                loot = generateLoot(lootModels[new Random().Next(0, lootModels.Count)]);
            }
            return loot;
        }
    }
}
