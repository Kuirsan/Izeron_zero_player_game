using System;
using System.Collections.Generic;
using System.Text;
using SomeKindOfGame.Interfaces;

namespace SomeKindOfGame.Persons
{
    abstract class AbstractPersonTier0:AbstractPerson,IXPRecievable
    {
        private protected int _lvlCup;
        private float _lvlMultiple = 1;
        private float _curXP = 0f;
        private protected Dictionary<int, float> _lvlTable;
        private protected AbstractPersonTier0(int HP, int LVL, string Name, Dictionary<int,float> LVLTable) : base(HP, LVL, Name)
        {
            _lvlTable = LVLTable;
            _lvlCup = _lvlTable.Count;
        }
        private void _gainXP(float Amount)
        {
            if (_lvl >= _lvlTable.Count) return;
            if (_curXP + (Amount*_lvlMultiple) >= _lvlTable[_lvl])
            {
                _lvlUp();
            }
            else
            {
                _curXP += Amount;
            }
        }
        private void _lvlUp() 
        {
            _lvl++;
            _curXP = 0;
        }
        public void ReceiveXP(float Amount)
        {
            _gainXP(Amount);
        }
    }
}
