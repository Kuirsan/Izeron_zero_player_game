using System;
using System.Collections.Generic;
using System.Text;
using SomeKindOfGame.Interfaces;

namespace SomeKindOfGame.Persons.Tier0
{
    class Peasant:AbstractPersonTier0,IDmgable
    {
        private float _attack;
        private int _str;
        private Peasant(Dictionary<int, float> LVLTable) :base(2,0,"Peasant",LVLTable)
        {
        }
        public Peasant(int Str, Dictionary<int, float> LVLTable) :this(LVLTable)
        {
            _str = Str;
            _attack = (int)(Str * 1.1f);
        }
        public override void MakeDmg(IDmgable dmgable)
        {
            dmgable.GetDamage((int)_attack);
        }

        public void GetDamage(int Amount)
        {
            _health -= Amount;
        }

    }
}
