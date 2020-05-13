using Izeron.Library.Interfaces;
using System.Collections.Generic;

namespace Izeron.Library.Persons.Tier0
{
    /// <summary>
    /// Tier 0 class - Peasant
    /// </summary>
    public class Peasant : AbstractPersonTier0, IDmgable
    {

        public float Attack => _str * 1.1f;

        private int _str;
        private Peasant(Dictionary<int, float> LVLTable) : base(2, 0, "Peasant", LVLTable)
        {
            //TODO rewrite
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
    }
}
