using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public abstract class BaseQuestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool isProfessionQuest { get; set; } = false;
        public bool isBaseQuest { get; set; } = false;
        public int? ParentID { get; set; }

        public bool isFinish = false;

        public BaseQuestModel(string title,string description)
        {
            Title = title;
            Description = description;
        }

        public abstract void getReward(AbstractPerson pers);

        public abstract void UpdateQuest();
        
    }
}
