﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class RewardModel
    {
        public int XpReward { get; set; }
        public int GoldReward { get; set; }
        public override string ToString()
        {
            return @$"
                      ||============||
                      ||___Опыт = {XpReward}_____||
                      ||___Золото = {GoldReward}___||
                      ||============||
                    ";
        }

        public RewardModel()
        {

        }
    }
}
