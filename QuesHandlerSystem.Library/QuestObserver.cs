using GameLogic.Library.GameBattleRoster;
using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using QuestHandlerSystem.Library.Quest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using GameLogic.Library.GameBattleRoster;

namespace QuestHandlerSystem.Library
{
    public class QuestObserver : IUpdatable
    {
        private List<BaseQuestModel> _activeQuests;
        private AbstractPerson _hero;
        private List<QuestLoadModel> _questsModels;
        private int _maxActiveQuests;
        private BattleRosterManager _monsterManager;
        private Func<int> _getCurrentFloor;

        public QuestObserver(AbstractPerson Hero,BattleRosterManager monsterManager, Func<int> getCurrentFloor = null)
        {
            _activeQuests = new List<BaseQuestModel>();
            _hero = Hero;
            LoadQuestModels();
            _maxActiveQuests = 5;
            _monsterManager = monsterManager;
            _getCurrentFloor = getCurrentFloor ?? (() => 1);
        }

        private void LoadQuestModels()
        {
            string fileName = @"QuestLibrary\quests.json";
            _questsModels = GetQuestModelsFromJSON(fileName);
        }

        public void ReloadQuestModels()
        {
            LoadQuestModels();
        }

        private List<QuestLoadModel> GetQuestModelsFromJSON(string path)
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<QuestLoadModel>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public void SignOnQuest(BaseQuestModel quest)
        {
            _activeQuests.Add(quest);
        }

        public int ActiveQuests()
        {
            return _activeQuests.Count;
        }

        public BaseQuestModel GenerateQuest()
        {
            // Если квесты закончились, генерируем случайный квест
            if (_questsModels.Count == 0)
            {
                return GenerateRandomQuest();
            }
            var questModel = _questsModels[0];
            
            // Проверяем тип квеста
            string questType = questModel.QuestType ?? "Kill";
            
            BaseQuestModel quest;
            if (questType.Equals("CollectLoot", StringComparison.OrdinalIgnoreCase))
            {
                List<AbstractPerson> monsters;
                if (questModel.Enemies != null && questModel.Enemies.Length > 0)
                {
                    monsters = _monsterManager.GenerateMonstersByName(questModel.Enemies);
                }
                else
                {
                    monsters = _monsterManager.GenerateRandomMonsters(_getCurrentFloor(), 10);
                }
                _monsterManager.AddMonsterToRoster(_getCurrentFloor(), monsters.ToArray());

                // Создаем квест на сбор лута
                quest = new CollectLootQuest(
                    questModel.Title,
                    questModel.Description,
                    questModel.RequiredLootName,
                    questModel.RequiredLootQuantity,
                    questModel.Reward,
                    _hero
                );
            }
            else
            {
                // Создаем обычный квест на убийство
                var monsters = _monsterManager.GenerateMonstersByName(questModel.Enemies);
                _monsterManager.AddMonsterToRoster(_getCurrentFloor(), monsters.ToArray());
                quest = new KillQuest(questModel.Title, questModel.Description, monsters, questModel.Reward);
            }

            _questsModels.Remove(questModel);
            return quest;
        }

        public BaseQuestModel GenerateQuestByType(string type)
        {
             // Try to find in models
             var questModel = _questsModels.FirstOrDefault(q => (q.QuestType ?? "Kill").Equals(type, StringComparison.OrdinalIgnoreCase));
             
             if (questModel != null)
             {
                 _questsModels.Remove(questModel);
                 if (type.Equals("CollectLoot", StringComparison.OrdinalIgnoreCase))
                 {
                    List<AbstractPerson> monsters;
                    if (questModel.Enemies != null && questModel.Enemies.Length > 0)
                    {
                        monsters = _monsterManager.GenerateMonstersByName(questModel.Enemies);
                    }
                    else
                    {
                        monsters = _monsterManager.GenerateRandomMonsters(_getCurrentFloor(), 10);
                    }
                    _monsterManager.AddMonsterToRoster(_getCurrentFloor(), monsters.ToArray());

                    return new CollectLootQuest(
                        questModel.Title,
                        questModel.Description,
                        questModel.RequiredLootName,
                        questModel.RequiredLootQuantity,
                        questModel.Reward,
                        _hero
                    );
                 }
                 else
                 {
                    var monsters = _monsterManager.GenerateMonstersByName(questModel.Enemies);
                    _monsterManager.AddMonsterToRoster(_getCurrentFloor(), monsters.ToArray());
                    return new KillQuest(questModel.Title, questModel.Description, monsters, questModel.Reward);
                 }
             }

             // If not found, generate random
             if (type.Equals("CollectLoot", StringComparison.OrdinalIgnoreCase))
             {
                 return GenerateRandomCollectQuest();
             }
             else
             {
                 return GenerateRandomKillQuest();
             }
        }

        private BaseQuestModel GenerateRandomQuest()
        {
            var r = new Random();
            if (r.Next(0, 2) == 0)
            {
                return GenerateRandomKillQuest();
            }
            else
            {
                return GenerateRandomCollectQuest();
            }
        }

        private BaseQuestModel GenerateRandomCollectQuest()
        {
            var lootNames = new[] { "Branch", "Iron Ingot", "Wolf Pelt", "Goblin Ear", "Precious Gem", "Dragon Scale", "Ancient Rune" };
            var r = new Random();
            var lootName = lootNames[r.Next(lootNames.Length)];
            var quantity = r.Next(1, 6);
            
            var title = $"Collect {quantity} {lootName}s";
            var description = $"We need {lootName}s for the town!";
            
            var reward = new RewardModel
            {
                GoldReward = 8 * quantity,
                XpReward = 8 * quantity
            };

            // Spawn monsters to drop loot (с учетом силы героя)
            int heroPower = HeroPowerCalculator.CalculatePowerRating(_hero);
            var monsters = _monsterManager.GenerateRandomMonstersByPower(_getCurrentFloor(), 10, heroPower);
            _monsterManager.AddMonsterToRoster(_getCurrentFloor(), monsters.ToArray());

            return new CollectLootQuest(title, description, lootName, quantity, reward, _hero);
        }

        private BaseQuestModel GenerateRandomKillQuest()
        {
            int heroPower = HeroPowerCalculator.CalculatePowerRating(_hero);
            var monsters = _monsterManager.GenerateRandomMonstersByPower(_getCurrentFloor(), 10, heroPower);
            var title = $"Monsters {GenerateVerb()} {GenerateSubject()}!";
            var description = "You must kill them!";
            var reward = new RewardModel
            {
                GoldReward = 10 + (monsters.Count * 2),
                XpReward = 10 + (monsters.Count * 2)
            };
            var quest = new KillQuest(title,description,monsters, reward);

            _monsterManager.AddMonsterToRoster(_getCurrentFloor(), monsters.ToArray());

            return quest;
        }

        private string GenerateVerb()
        {
            var r = new Random(DateTime.Now.AddMilliseconds(100).Millisecond).Next(1, 4);
            var verb =  r switch 
            {
                1 => "ate",
                2 => "stole",
                3 => "ruined",
                _=>"default"
            };
            return verb;
        }

        private string GenerateSubject()
        {
        var r = new Random(DateTime.Now.Millisecond).Next(1, 4);
        var subj = r switch
            {
                1 => "cake",
                2 => "plates",
                3 => "king's soul",
                _ => "default"
            };
            return subj;
        }

        protected string UpdateAllQuests()
        {
            string notification = string.Empty;
            var activeQuest = _activeQuests.Where(quest => !quest.IsFinish).ToArray();
            foreach (var quest in activeQuest)
            {
                notification+= UpdateQuest(quest);
            }
            RemoveObsoleteQuests();
            return notification;
        }

        protected void RemoveObsoleteQuests()
        {
            foreach(var quest in _activeQuests)
            {
                if(quest.IsFinish) quest.GetReward(_hero);
            }
            _activeQuests.RemoveAll(quest => quest.IsFinish);
        }

        protected static string UpdateQuest(BaseQuestModel quest)
        {
            return quest.UpdateQuest();
        }

        public GameNotification Update()
        {
            GameNotification gameNotification = new GameNotification()
            {
                GameNotificationState = GameNotificationState.Quest
            };
            gameNotification.Body = UpdateAllQuests();
            return gameNotification;
        }

        public void UpdateQuestListFromChildQuests(BaseQuestModel[] childQuests)
        {
            _activeQuests.AddRange(childQuests);
        }

        public List<BaseQuestModel> GetActiveQuests()
        {
            return _activeQuests.Where(quest => !quest.IsFinish).ToList();
        }
    }
}
