using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Persons.Enemies.Tier0
{
    public class Rat : AbstractPerson,IDmgable,IXPTransmittable
    {
        protected int _xpToGain;
        private protected int _str;

        public Rat(int HP, int LVL, string Name,int str) : base(HP, LVL, Name)
        {

            _str = str;
            _xpToGain = HP;
        }

        public int Attack => _str;

        public override int AttackAmount()
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

        public void TransmitXP(IXPRecievable recievable)
        {
            recievable.ReceiveXP(_xpToGain);
        }
    }
}
