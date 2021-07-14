using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Persons.Enemies
{
    public class Monster : AbstractPerson, IXPTransmittable,IDmgable
    {
        private int _attack;
        private int _xpToGain;

        public Monster(EnemyFillModel model):base(model.HP,1,model.Name)
        {
            _attack = model.Attack;
            _xpToGain = model.XPToGain;
        }

        public override int attackAmount()
        {
            return _attack;
        }

        public override void MakeDmg(IDmgable dmgable)
        {
            dmgable.GetDamage(attackAmount());
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

        public void TransmitXP(IXPRecievable recievable)
        {
            recievable.ReceiveXP(_xpToGain);
        }
    }
}
