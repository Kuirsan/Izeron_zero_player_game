using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Persons.Enemies.Tier0
{
    public class Rat : AbstractPerson,IDmgable
    {
        public Rat(int HP, int LVL, string Name,float str) : base(HP, LVL, Name)
        {

            _str = str;
        }

        public float Attack => _str;

        private protected float _str;

        public override float attackAmount()
        {
            return Attack;
        }

        public void GetDamage(int Amount)
        {
            if (_isDead)
            {
                Death();
            }
            else
            {
                _health -= Amount;
                if (_health <= 0)
                {
                    _health = 0;
                    Death();
                }
            }
        }

        public override void MakeDmg(IDmgable dmgable)
        {
            dmgable.GetDamage((int)Attack);
        }
        protected override void Death()
        {
            _isDead = true;
        }
    }
}
