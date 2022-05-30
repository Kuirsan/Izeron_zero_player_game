using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using QuestHandlerSystem.Library.Quest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestHandlerSystem.Library
{
    public class QuestObserver : IUpdatable
    {
        private readonly List<BaseQuestModel> _activeQuests;
        private readonly AbstractPerson _hero;

        public QuestObserver(AbstractPerson Hero)
        {
            _activeQuests = new List<BaseQuestModel>();
            _hero = Hero;
        }

        public void SignOnQuest(BaseQuestModel quest)
        {
            _activeQuests.Add(quest);
        }

        protected string UpdateAllQuests()
        {
            string notification = string.Empty;
            foreach (var quest in _activeQuests.Where(quest => !quest.IsFinish))
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
    }
}
