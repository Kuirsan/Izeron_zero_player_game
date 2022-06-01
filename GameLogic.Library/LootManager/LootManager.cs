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
        public override void AddLoot(ILootable loot)
        {
            _lootableObjects.Enqueue(loot);
        }

        public override void GenerateLootAndAddByFloor(int floor, int lootCount)
        {
            for (int i = 0; i < lootCount; i++)
            {
                LootableBaseObject loot = GenerateRandomLoot(floor);
                AddLoot(loot);
            }
        }

        public override ILootable GetNextLootableObject()
        {
            if (_lootableObjects.Count == 0) return null;
            return _lootableObjects.Dequeue();
        }
        public LootManager() : base()
        {
            LoadEnemiesModels();
        }

        private void LoadEnemiesModels()
        {
            string fileName = @"LootsLibrary\loots.json";
            _lootsModels = GetLootsModelsFromJSON(fileName);
        }

        private List<GameLootModel> GetLootsModelsFromJSON(string path)
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<GameLootModel>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private LootableBaseObject GenerateLoot(GameLootModel lootModel)
        {
            LootFillModel lootFillModel = new LootFillModel
            {
                Cost = new Random().Next(lootModel.CostInGoldRange.MinParameter, lootModel.CostInGoldRange.MaxParameter),
                Name = lootModel.Name
            };

            return new CommonLoot(lootFillModel);
        }

        public LootableBaseObject GenerateRandomLoot(int floor)
        {
            LootableBaseObject loot = null;
            if (_lootsModels == null) return null;
            List<GameLootModel> lootModels = _lootsModels
                                                    .Where(x => x.FloorRange.MinParameter <= floor && x.FloorRange.MaxParameter >= floor)
                                                    .Select(x => x).ToList();
            if (lootModels.Count == 0) return null;
            if (lootModels.Count == 1) loot = GenerateLoot(lootModels.First());
            if (lootModels.Count > 1)
            {
                loot = GenerateLoot(lootModels[new Random().Next(0, lootModels.Count)]);
            }
            return loot;
        }
    }
}
