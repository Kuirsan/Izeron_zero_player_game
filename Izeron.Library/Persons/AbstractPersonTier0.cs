#nullable enable
using Izeron.Library.Interfaces;
using Izeron.Library.InventorySystem;
using Izeron.Library.Perks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Izeron.Library.Persons
{
    /// <summary>
    /// Base class for another persons tier 0
    /// </summary>
    public abstract class AbstractPersonTier0 : AbstractPerson, IXPRecievable, INotifyPropertyChanged
    {
        private protected int _lvlCup;
        private readonly float _lvlMultiple = 1;
        private float _curXP = 0f;
        private protected Dictionary<int, float> _lvlTable;
        private protected List<ActivePerk> _perks;
        
        public event EventHandler<int>? LeveledUp;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public int CurrentXP
        {
            get => (int)_curXP;
        }

        public int MaxXP 
        {
            get
            {
                if (_lvlTable.ContainsKey(_lvl))
                {
                    return (int)_lvlTable[_lvl];
                }
                else
                {
                    return (int)_lvlTable[_lvlCup];
                }
            }
        }

        private protected AbstractPersonTier0(int HP, int LVL, string Name, Dictionary<int, float> LVLTable,InventoryBase inventory) : base(HP, LVL, Name)
        {
            _lvlTable = LVLTable;
            _lvlCup = _lvlTable.Count-1;
            _inventory = inventory;
            _perks = new List<ActivePerk>();
        }
        /// <summary>
        /// Gain amount of XP to person. When XP>=MaxXP then lvlUp
        /// </summary>
        /// <param name="Amount">amont of xp gained</param>
        private void GainXP(float Amount)
        {
            if (_lvl >= _lvlCup) return;
            if (_curXP + (Amount * _lvlMultiple) >= _lvlTable[_lvl])
            {
                LvlUp();
            }
            else
            {
                _curXP += Amount;
            }
            OnPropertyChanged(nameof(CurrentXP));
        }
        /// <summary>
        /// lvlUp the character.
        /// </summary>
        protected virtual void LvlUp()
        {
            _lvl++;
            _curXP = 0;
            OnPropertyChanged(nameof(MaxXP));
            OnPropertyChanged(nameof(CharacterList));
            
            // Вызываем событие повышения уровня
            LeveledUp?.Invoke(this, _lvl);
        }
        /// <summary>
        /// Recieve amount of XP.
        /// </summary>
        /// <param name="Amount">How many XP get chracter</param>
        public void ReceiveXP(int Amount)
        {
            GainXP(Amount);
        }

        public override bool AddItemToInventory(ILootable item)
        {
            bool addedToInventory = _inventory.TryToAddItemToInventory(item);
            OnPropertyChanged(nameof(InventoryList));
            return addedToInventory;
        }
        public override bool SomethingInInventory()
        {
            return _inventory.SomethingInInventory(IsQuestItem);
        }

        public override void SellItemInInventory()
        {
            var item = _inventory.GetItemForSale(IsQuestItem);
            if (item == null) return;
            AddMoneyAmount(item.Volume);
            _inventory.TryToRemoveFromInventory(item);
            OnPropertyChanged(nameof(InventoryList));
        }
        public override void SetMoneyAmount(int value)
        {
            _money = value;
            OnPropertyChanged(nameof(CharacterList));
        }
        public override void AddMoneyAmount(int money)
        {
            base.AddMoneyAmount(money);
            OnPropertyChanged(nameof(CharacterList));
        }
        
        /// <summary>
        /// Добавить перк персонажу
        /// </summary>
        public virtual void AddPerk(ActivePerk perk)
        {
            var existingPerk = _perks.FirstOrDefault(p => p.Name == perk.Name);
            if (existingPerk != null)
            {
                // Если перк уже есть, стакаем его
                existingPerk.Stack();
                ApplyPerkStack(existingPerk);
            }
            else
            {
                // Если перка нет, добавляем новый
                _perks.Add(perk);
                ApplyPerk(perk);
            }
            OnPropertyChanged(nameof(PerksList));
        }
        
        /// <summary>
        /// Применить эффекты нового перка
        /// </summary>
        protected virtual void ApplyPerk(ActivePerk perk)
        {
            switch (perk.Type.ToLower())
            {
                case "health":
                    _maxHealth += perk.Value;
                    _health += perk.Value;
                    OnPropertyChanged(nameof(CharacterList));
                    break;
                // Остальные типы применяются динамически
            }
        }

        /// <summary>
        /// Применить эффекты от стака перка
        /// </summary>
        protected virtual void ApplyPerkStack(ActivePerk perk)
        {
            switch (perk.Type.ToLower())
            {
                case "health":
                    _maxHealth += perk.BaseValue;
                    _health += perk.BaseValue;
                    OnPropertyChanged(nameof(CharacterList));
                    break;
                // Остальные типы применяются динамически
            }
        }
        
        /// <summary>
        /// Получить бонус к урону от перков
        /// </summary>
        public int GetDamageBonus()
        {
            return _perks.Where(p => p.Type.Equals("Damage", StringComparison.OrdinalIgnoreCase))
                         .Sum(p => p.Value);
        }
        
        /// <summary>
        /// Получить бонус к защите от перков
        /// </summary>
        public int GetDefenseBonus()
        {
            return _perks.Where(p => p.Type.Equals("Defense", StringComparison.OrdinalIgnoreCase))
                         .Sum(p => p.Value);
        }
        
        /// <summary>
        /// Список перков для отображения
        /// </summary>
        public virtual List<PerkViewModel> PerksList
        {
            get
            {
                var perkList = new List<PerkViewModel>();
                foreach (var perk in _perks)
                {
                    perkList.Add(new PerkViewModel(perk));
                }
                return perkList;
            }
        }
    }
}
