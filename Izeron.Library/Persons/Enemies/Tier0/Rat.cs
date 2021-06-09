using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Persons.Enemies.Tier0
{
    public class Rat : AbstractPerson,IDmgable
    {
        public Rat(int HP, int LVL, string Name) : base(HP, LVL, Name)
        {
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
            dmgable.GetDamage(1);
        }
        protected override void Death()
        {
            _isDead = true;
        }
    }
}
