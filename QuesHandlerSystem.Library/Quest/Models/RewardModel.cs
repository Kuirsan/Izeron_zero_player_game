using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class RewardModel
    {
        public int XpReward;
        public int GoldReward;
        public override string ToString()
        {
            return @$"
                      Опыт = {XpReward}
                      Золото = {GoldReward}";

        }
    }
}
