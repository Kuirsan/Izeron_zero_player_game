using System;
using System.Collections.Generic;
using System.Text;
using SomeKindOfGame.Interfaces;
using SomeKindOfGame.Objects;

namespace SomeKindOfGame.Persons
{
    abstract class AbstractPerson:AbstractMainObject
    {
        private protected int _health;
        private protected int _lvl;
        public int Health
        {
            get
            {
                return _health;
            }
        }
        public AbstractPerson(int HP,int LVL,string Name):base(Name)
        {
            _health = HP;
            _lvl = LVL;
        }
        abstract public void MakeDmg(IDmgable dmgable);

    }
}
