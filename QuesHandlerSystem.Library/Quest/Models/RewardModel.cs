using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class RewardModel
    {
        public int xpReward { get; set; }
        public int goldReward { get; set; }
        public override string ToString()
        {
            return @$"
                      Опыт = {xpReward}
                      Золото = {goldReward}";

        }
    }
}
