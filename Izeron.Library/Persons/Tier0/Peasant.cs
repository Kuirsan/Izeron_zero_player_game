﻿using Izeron.Library.Enums;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.InventorySystem;
using System;
using System.Collections.Generic;

namespace Izeron.Library.Persons.Tier0
{
    /// <summary>
    /// Tier 0 class - Peasant
    /// </summary>
    public class Peasant : AbstractPersonTier0, IDmgable,IHealable
    {
        private protected int _attackModifier = 0;
        public int Attack => _str  + _attackModifier;

        private protected int _str;
        private Peasant(Dictionary<int, float> LVLTable) : base(5, 0, "Peasant", LVLTable,new InventoryPerson(15))
        {
            //TODO rewrite
            _personTags.Add(PersonTags.Human);
        }
        public Peasant(int Str, Dictionary<int, float> LVLTable) : this(LVLTable)
        {
            //TODO rewrite
            _str = Str;

        }
        /// <summary>
        /// Make damage to instance of IDmgable
        /// </summary>
        /// <param name="dmgable"></param>
        public override void MakeDmg(IDmgable dmgable)
        {
            dmgable.GetDamage(Attack);
        }
        /// <summary>
        /// Get amount of damage
        /// </summary>
        /// <param name="Amount"></param>
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
            OnPropertyChanged("CharacterList");
        }

        protected override void Death()
        {
            _isDead = true;
            throw new YouDeadException("You DIED");
        }

        public override Dictionary<string, string> CharacterList
        {
            get
            {
                Dictionary<string, string> valPairs = base.CharacterList;
                valPairs.Add("Урон", Attack.ToString());
                valPairs.Add("Монеты", Money.ToString());
                return valPairs;
            }
        }
        protected override void _lvlUp()
        {
            _str += (Int32)(new Random().NextDouble()+0.020f*_lvl* _str);
            _maxHealth += new Random().Next((int)(1 + 0.015f * _lvl), (int)(3*(1 + 0.015f * _lvl)));
            base._lvlUp();
        }

        public void getHeal(int amount)
        {
            if (amount < 0) return;
            if (_health + amount > _maxHealth)
            {
                _health = _maxHealth;
            }
            else
            {
                _health += amount;
            }
            OnPropertyChanged("CharacterList");
        }

        public override int attackAmount()
        {
            return Attack;
        }
    }
}
