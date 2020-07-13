using Izeron.Library.Interfaces;
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
            get => (int)_lvlTable[_lvl];
        }

        private protected AbstractPersonTier0(int HP, int LVL, string Name, Dictionary<int, float> LVLTable) : base(HP, LVL, Name)
        {
            _lvlTable = LVLTable;
            _lvlCup = _lvlTable.Count;
        }
        /// <summary>
        /// Gain amount of XP to person. When XP>=MaxXP then lvlUp
        /// </summary>
        /// <param name="Amount">amont of xp gained</param>
        private void _gainXP(float Amount)
        {
            if (_lvl >= _lvlTable.Count) return;
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
        public void ReceiveXP(float Amount)
        {
            _gainXP(Amount);
        }
    }
}
