using Izeron.Library.Interfaces;
using Izeron.Library.InventorySystem;
using System.Collections.Generic;
using System.ComponentModel;
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

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null)
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
            return _inventory.SomethingInInventory();
        }

        public override void SellItemInInventory()
        {
            var item = _inventory.GetItemForSale();
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
    }
}
