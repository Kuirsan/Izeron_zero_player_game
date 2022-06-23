using Izeron.Library.Enums;
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
        private protected HashSet<SpecialEnemyTags> _enemyTags;
        private protected HashSet<PersonTags> _enemyTypeTags;
        private string _prefix = string.Empty;
        public Monster(EnemyFillModel model):base(model.HP,1,model.Name)
        {
            _attack = model.Attack;
            _xpToGain = model.XPToGain;
            _enemyTags = model.EnemyTags;
            _enemyTypeTags = model.EnemyTypeTags;
            InitiateTags();
        }

        //TODO
        private void InitiateTags()
        {
            float expMultiplier = 1f;
            foreach(var tag in _enemyTags)
            {
                if (tag == SpecialEnemyTags.Big)
                {
                    _health = (int)(_maxHealth * 1.3f);
                    _maxHealth = (int)(_maxHealth * 1.3f);
                    _prefix += "Big ";
                    expMultiplier += 0.3f;
                }
                if (tag == SpecialEnemyTags.Small)
                {
                    _health = (int)(_maxHealth * 0.3f);
                    _maxHealth = (int)(_maxHealth * 0.3f);
                    _prefix += "Small ";
                    expMultiplier -= 0.3f;
                }
                if (tag == SpecialEnemyTags.Strong)
                {
                    _attack++;
                    _prefix += "Strong ";
                    expMultiplier += 0.3f;
                }
                if (tag == SpecialEnemyTags.Weak)
                {
                    _attack =_attack==1 ? 1 : --_attack;
                    _prefix += "Weak ";
                    expMultiplier -= 0.3f;
                }
            }
            _xpToGain = (int)(_xpToGain * expMultiplier);
        }

        public override int AttackAmount()
        {
            return _attack;
        }

        public override void MakeDmg(IDmgable dmgable)
        {
            dmgable.GetDamage(AttackAmount());
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
        public override string ToString()
        {
            return $"{_prefix}{base.ToString()}";
        }
    }
}
