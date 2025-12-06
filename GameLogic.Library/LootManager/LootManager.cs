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
        
        // Делегат для проверки, является ли предмет квестовым
        public Func<string, bool> IsQuestItem { get; set; }

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

            // 1. Сначала ищем квестовые предметы
            List<GameLootModel> questLootModels = new List<GameLootModel>();
            if (IsQuestItem != null)
            {
                questLootModels = _lootsModels.Where(x => IsQuestItem(x.Name)).ToList();
            }

            // 2. Если есть квестовые предметы, с 50% шансом пытаемся выбрать один из них
            // Игнорируем требования по этажу для квестовых предметов
            if (questLootModels.Count > 0 && new Random().NextDouble() < 0.5)
            {
                return GenerateLoot(questLootModels[new Random().Next(questLootModels.Count)]);
            }

            // 3. Обычная генерация по этажу
            List<GameLootModel> lootModels = _lootsModels
                                                    .Where(x => x.FloorRange.MinParameter <= floor && x.FloorRange.MaxParameter >= floor)
                                                    .Select(x => x).ToList();
            
            // Fallback если ничего не найдено для этажа
            if (lootModels.Count == 0 && questLootModels.Count == 0) return null;
            
            // Если для этажа нет лута, но есть квестовый (который не выпал по шансу выше), берем квестовый
            if (lootModels.Count == 0 && questLootModels.Count > 0)
            {
                 return GenerateLoot(questLootModels[new Random().Next(questLootModels.Count)]);
            }
            else if (lootModels.Count == 0)
            {
                 // Если вообще ничего нет
                 return null;
            }

            if (lootModels.Count == 1) loot = GenerateLoot(lootModels.First());
            if (lootModels.Count > 1)
            {
                loot = GenerateLoot(lootModels[new Random().Next(0, lootModels.Count)]);
            }
            return loot;
        }
    }
}
