using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public abstract class BaseQuestModel
    {
        protected BaseQuestModel[] _childQuests;

        private bool _isBlockedByParent;
        public string Title { get; set; }

        public string Description { get; set; }

        public bool isFinish { get; set; }

        public BaseQuestModel(string title,string description)
        {
            Title = title;
            Description = description;
        }

        public BaseQuestModel(string title,string description, BaseQuestModel[] childQuests):this(title,description)
        {
            _childQuests = childQuests;
        }

        public virtual bool isAvailable()
        {
            return !_isBlockedByParent;
        }

        public virtual void unBlockQuest()
        {
            _isBlockedByParent = false;
        }

        public virtual void BlockQuest()
        {
            _isBlockedByParent = true;
        }

        public abstract void getReward(AbstractPerson pers);

        public abstract string UpdateQuest();


    }
}
