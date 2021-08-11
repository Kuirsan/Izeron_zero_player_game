using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using System.Collections.Generic;

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
            pers.addMoneyAmount(_reward.goldReward);
        }

        public override string UpdateQuest()
        {
            string notification=string.Empty;
            if (isFinish) return string.Empty;
            if (_enemies.Count == 0)
            {
                isFinish = true;
                if (_childQuests != null)
                {
                    foreach (var quest in _childQuests) quest.unBlockQuest();
                }
                notification=setNotificationText();
            }
            else
            {
                _enemies.RemoveAll(enemy => enemy.isDead());
            }
            return notification;
        }
        private string setNotificationText()
        {
            return $@"Квест {Title} завершен! Награда: {_reward}";
        }
    }
}
