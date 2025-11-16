using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using System.Linq;

namespace QuestHandlerSystem.Library.Quest.Models
{
    public class CollectLootQuest : BaseQuestModel
    {
        private readonly string _requiredLootName;
        private readonly int _requiredQuantity;
        private readonly RewardModel _reward;
        private readonly AbstractPerson _hero;

        public CollectLootQuest(string title, string description, string requiredLootName, int requiredQuantity, RewardModel reward, AbstractPerson hero) 
            : base(title, description)
        {
            _requiredLootName = requiredLootName;
            _requiredQuantity = requiredQuantity;
            _reward = reward;
            _hero = hero;
        }

        public override string ProgressInfo
        {
            get
            {
                if (IsFinish) return "Завершен";
                var inventoryList = _hero.InventoryList;
                var lootItem = inventoryList.FirstOrDefault(item => item.Name == _requiredLootName);
                int collectedQuantity = lootItem?.Qty ?? 0;
                return $"{collectedQuantity}/{_requiredQuantity} {_requiredLootName}";
            }
        }

        public CollectLootQuest(string title, string description, string requiredLootName, int requiredQuantity, RewardModel reward, AbstractPerson hero, 
            BaseQuestModel[] childQuests, UpdateQuestListHandle updateQuestListHandle) 
            : base(title, description, childQuests, updateQuestListHandle)
        {
            _requiredLootName = requiredLootName;
            _requiredQuantity = requiredQuantity;
            _reward = reward;
            _hero = hero;
        }

        public override void GetReward(AbstractPerson pers)
        {
            if (pers is IXPRecievable hero)
            {
                hero.ReceiveXP(_reward.XpReward);
            }
            pers.AddMoneyAmount(_reward.GoldReward);
        }

        public override string UpdateQuest()
        {
            string notification = string.Empty;
            if (IsFinish) return string.Empty;

            // Проверяем инвентарь героя на наличие нужного количества лута
            var inventoryList = _hero.InventoryList;
            var lootItem = inventoryList.FirstOrDefault(item => item.Name == _requiredLootName);
            
            int collectedQuantity = lootItem?.Qty ?? 0;

            if (collectedQuantity >= _requiredQuantity)
            {
                IsFinish = true;
                if (_childQuests != null)
                {
                    InvokeNotifyQuestListSystem();
                }
                notification = SetNotificationText();
            }

            return notification;
        }

        private string SetNotificationText()
        {
            return $@"Квест {Title} завершен! Награда: {_reward}";
        }
    }
}

