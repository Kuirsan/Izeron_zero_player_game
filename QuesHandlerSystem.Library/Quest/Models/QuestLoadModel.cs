using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class QuestLoadModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string[] Enemies { get; set; }

        public RewardModel Reward {get;set;}
    }
}
