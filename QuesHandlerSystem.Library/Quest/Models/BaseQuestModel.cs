using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public abstract class BaseQuestModel
    {
        private protected Guid _questGuid;
        public string Title { get; set; }
        public string Description { get; set; }
        public bool isProfessionQuest { get; set; } = false;
        public bool isBaseQuest { get; set; } = false;
        public string ParentQuestGuid { get; set; }

        public string QuestGuid => this.ToString();

        public bool isFinish = false;

        public BaseQuestModel(string title,string description)
        {
            Title = title;
            Description = description;
            _questGuid = getGuid();
        }

        public abstract void getReward(AbstractPerson pers);

        public abstract string UpdateQuest();

        public override string ToString()
        {
            return _questGuid.ToString();
        }

        private Guid getGuid()
        {
            using MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(Title + Description));
            return new Guid(hash);
        }

    }
}
