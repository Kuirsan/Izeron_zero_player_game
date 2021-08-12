﻿using Izeron.Library.Enums;
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
        private List<BaseQuestModel> _activeQuests;
        private AbstractPerson _hero;

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
            var activeQuest = _activeQuests.Where(quest => !quest.isFinish).ToArray();
            foreach (var quest in activeQuest)
            {
                notification+=UpdateQuest(quest);
            }
            RemoveObsoleteQuests();
            return notification;
        }

        protected void RemoveObsoleteQuests()
        {
            foreach(var quest in _activeQuests)
            {
                if(quest.isFinish) quest.getReward(_hero);
            }
            _activeQuests.RemoveAll(quest => quest.isFinish);
        }

        protected string UpdateQuest(BaseQuestModel quest)
        {
            return quest.UpdateQuest();
        }

        public GameNotification Update()
        {
            GameNotification gameNotification = new GameNotification()
            {
                gameNotificationState = GameNotificationState.Quest
            };
            gameNotification.body = UpdateAllQuests();
            return gameNotification;
        }

        public void updateQuestListFromChildQuests(BaseQuestModel[] childQuests)
        {
            _activeQuests.AddRange(childQuests);
        }
    }
}
