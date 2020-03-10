using System;
using System.Collections.Generic;
using System.Text;

namespace QuesHandlerSystem.Library.Quest.Models
{
    public class QuestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool isProfessionQuest { get; set; }
        public bool isBaseQuest { get; set; }
        public int ParentID { get; set; }
    }
}
