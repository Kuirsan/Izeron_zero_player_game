using QuesHandlerSystem.Library.Quest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuesHandlerSystem.Library
{
    public class QuestObserver
    {
        private List<QuestModel> activeQuests;

        public QuestObserver()
        {
            activeQuests = new List<QuestModel>();
        }

        public void SignOnQuest(QuestModel quest)
        {
            activeQuests.Add(quest);
        }
        public void RemoveQuest(QuestModel quest)
        {
            activeQuests.Remove(quest);
        }

        public void UpdateAllQuests()
        {
            //TODO all quests update
        }

        public void UpdateQuest(QuestModel quest)
        {
            //TODO single quest update
        }

    }
}
