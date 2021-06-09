using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using QuesHandlerSystem.Library.Quest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class KillQuest : BaseQuestModel
    {
        private List<AbstractPerson> _enemies;
        private RewardModel _reward;

        public KillQuest(string title,string description, List<AbstractPerson> enemies,RewardModel reward):base(title,description)
        {
            _enemies = enemies;
            _reward = reward;
        }
        public override void getReward(AbstractPerson pers)
        {
            if(pers is IXPRecievable hero)
            {
                hero.ReceiveXP(_reward.xpReward);
            }
        }

        public override void UpdateQuest()
        {
            if (isFinish) return;
            if (_enemies.Count == 0)
            {
                isFinish = true;
            }
            else
            {
                _enemies.RemoveAll(enemy => enemy.isDead());
            }
        }
    }
}
