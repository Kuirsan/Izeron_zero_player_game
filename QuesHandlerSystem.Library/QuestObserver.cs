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
            if (_questsModels.Count == 0) throw new Exception("There is now quest models!");
            var questModel = _questsModels[0];
            var monsters = _monsterManager.GenerateMonstersByName(questModel.Enemies);

            _monsterManager.AddMonsterToRoster(1, monsters.ToArray());
            var quest = new KillQuest(questModel.Title, questModel.Description, monsters, questModel.Reward);

            _questsModels.Remove(questModel);
            return quest;
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
    }
}
