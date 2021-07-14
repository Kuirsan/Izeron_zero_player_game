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
        private float _lvlMultiple = 1;
        private float _curXP = 0f;
        private protected Dictionary<int, float> _lvlTable;
        private protected InventoryBase _inventory;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
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
        private void _gainXP(float Amount)
        {
            if (_lvl >= _lvlCup) return;
            if (_curXP + (Amount * _lvlMultiple) >= _lvlTable[_lvl])
            {
                _lvlUp();
            }
            else
            {
                _curXP += Amount;
            }
            OnPropertyChanged("CurrentXP");
        }
        /// <summary>
        /// lvlUp the character.
        /// </summary>
        protected virtual void _lvlUp()
        {
            _lvl++;
            _curXP = 0;
            OnPropertyChanged("MaxXP");
            OnPropertyChanged("CharacterList");
        }
        /// <summary>
        /// Recieve amount of XP.
        /// </summary>
        /// <param name="Amount">How many XP get chracter</param>
        public void ReceiveXP(int Amount)
        {
            _gainXP(Amount);
        }

        public override bool AddItemToInventory(ILootable item)
        {
            return _inventory.tryToAddItemToInventory(item);
        }
        public override bool somethingInInventory()
        {
            return _inventory.somethingInInventory();
        }

        public override void sellIteminInventory()
        {
            var item = _inventory.getItemForSale();
            addMoneyAmount(item.Volume);
            _inventory.tryToRemoveFromInventory(item);
        
        }
        public override void setMoneyAmount(int value)
        {
            _money = value;
            OnPropertyChanged("CharacterList");
        }
        public override void addMoneyAmount(int money)
        {
            base.addMoneyAmount(money);
            OnPropertyChanged("CharacterList");
        }
    }
}
