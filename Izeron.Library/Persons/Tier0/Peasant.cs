using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;

namespace Izeron.Library.Persons.Tier0
{
    /// <summary>
    /// Tier 0 class - Peasant
    /// </summary>
    public class Peasant : AbstractPersonTier0, IDmgable
    {
        private protected decimal _attackModifier = 0.0M;
        public decimal Attack => (_str * 1.1M) + _attackModifier;

        private protected int _str;
        private Peasant(Dictionary<int, float> LVLTable) : base(2, 0, "Peasant", LVLTable)
        {
            //TODO rewrite
            _personTags.Add(Enums.PersonTags.Human);
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
            dmgable.GetDamage((int)Attack);
        }
        /// <summary>
        /// Get amount of damage
        /// </summary>
        /// <param name="Amount"></param>
        public void GetDamage(int Amount)
        {
            _health -= Amount;
            if (_health <= 0)
            {
                Death();
            }
            OnPropertyChanged("CharacterList");
        }

        protected virtual void Death()
        {
            //TODO some logic
        }

        public override Dictionary<string, string> CharacterList
        {
            get
            {
                Dictionary<string, string> valPairs = base.CharacterList;
                valPairs.Add("Урон", Attack.ToString());
                return valPairs;
            }
        }
        protected override void _lvlUp()
        {
            _str += (Int32)(new Random().NextDouble()+0.015f*_lvl);
            _maxHealth += new Random().Next(1,3);
            base._lvlUp();
        }
    }
}
