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

namespace QuestHandlerSystem.Library
{
    public class QuestObserver : IUpdatable
    {
        private List<BaseQuestModel> _activeQuests;
        private AbstractPerson _hero;
        private List<QuestLoadModel> _questsModels;
        private int _maxActiveQuests;
        private BattleRosterManager _monsterManager;

        public QuestObserver(AbstractPerson Hero,BattleRosterManager monsterManager)
        {
            _activeQuests = new List<BaseQuestModel>();
            _hero = Hero;
            LoadQuestModels();
            _maxActiveQuests = 5;
            _monsterManager = monsterManager;
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
            // Если квесты закончились, перезагружаем их из JSON
            if (_questsModels.Count == 0)
            {
                ReloadQuestModels();
                // Если после перезагрузки все еще пусто, генерируем случайный квест на убийство
                if (_questsModels.Count == 0)
                {
                    return GenerateRandomKillQuest();
                }
            }
            var questModel = _questsModels[0];
            
            // Проверяем тип квеста
            string questType = questModel.QuestType ?? "Kill";
            
            BaseQuestModel quest;
            if (questType.Equals("CollectLoot", StringComparison.OrdinalIgnoreCase))
            {
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
                _monsterManager.AddMonsterToRoster(1, monsters.ToArray());
                quest = new KillQuest(questModel.Title, questModel.Description, monsters, questModel.Reward);
            }

            _questsModels.Remove(questModel);
            return quest;
        }

        private BaseQuestModel GenerateRandomKillQuest()
        {
            var monsters = _monsterManager.GenerateRandomMonsters(1, 10);
            var title = $"Monsters {GenerateVerb()} {GenerateSubject()}!";
            var description = "You must kill them!";
            var reward = new RewardModel
            {
                GoldReward = 15,
                XpReward = 15
            };
            var quest = new KillQuest(title,description,monsters, reward);

            _monsterManager.AddMonsterToRoster(1, monsters.ToArray());

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
