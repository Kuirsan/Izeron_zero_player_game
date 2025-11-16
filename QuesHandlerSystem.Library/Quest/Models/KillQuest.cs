using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using System.Collections.Generic;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class KillQuest : BaseQuestModel
    {
        private readonly List<AbstractPerson> _enemies;
        private readonly RewardModel _reward;

        public KillQuest(string title,string description, List<AbstractPerson> enemies,RewardModel reward) :base(title,description)
        {
            _enemies = enemies;
            _reward = reward;
        }

        public override string ProgressInfo
        {
            get
            {
                if (IsFinish) return "Завершен";
                return $"Осталось врагов: {_enemies.Count}";
            }
        }

        public KillQuest(string title, string description, List<AbstractPerson> enemies, RewardModel reward, BaseQuestModel[] childQuests, UpdateQuestListHandle updateQuestListHandle) :base(title, description, childQuests, updateQuestListHandle)
        {
            _enemies = enemies;
            _reward = reward;
        }
        public override void GetReward(AbstractPerson pers)
        {
            if(pers is IXPRecievable hero)
            {
                hero.ReceiveXP(_reward.XpReward);
            }
            pers.AddMoneyAmount(_reward.GoldReward);
        }

        public override string UpdateQuest()
        {
            string notification=string.Empty;
            if (IsFinish) return string.Empty;
            if (_enemies.Count == 0)
            {
                IsFinish = true;
                if (_childQuests != null)
                {
                    InvokeNotifyQuestListSystem();
                }
                notification=SetNotificationText();
            }
            else
            {
                _enemies.RemoveAll(enemy => enemy.IsDead());
            }
            return notification;
        }
        private string SetNotificationText()
        {
            return $@"Квест {Title} завершен! Награда: {_reward}";
        }
    }
}
