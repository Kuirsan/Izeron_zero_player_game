using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public abstract class BaseQuestModel
    {
        public delegate void UpdateQuestListHandle(BaseQuestModel[] childQuests);
        public event UpdateQuestListHandle NotifyQuestListSystem;

        protected BaseQuestModel[] _childQuests;

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsFinish = false;

        public abstract string ProgressInfo { get; }

        public BaseQuestModel(string title,string description)
        {
            Title = title;
            Description = description;
        }

        public BaseQuestModel(string title,string description, BaseQuestModel[] childQuests, UpdateQuestListHandle updateQuestListHandle) :this(title,description)
        {
            _childQuests = childQuests;
            NotifyQuestListSystem += updateQuestListHandle;
        }

        public abstract void GetReward(AbstractPerson pers);

        public abstract string UpdateQuest();

        protected void InvokeNotifyQuestListSystem()
        {
            NotifyQuestListSystem?.Invoke(_childQuests);
            _childQuests = null;
        }

    }
}
