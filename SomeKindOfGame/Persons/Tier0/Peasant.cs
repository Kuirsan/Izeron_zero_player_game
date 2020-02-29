using System;
using System.Collections.Generic;
using System.Text;
using SomeKindOfGame.Interfaces;

namespace SomeKindOfGame.Persons.Tier0
{
    class Peasant:AbstractPersonTier0,IDmgable
    {
        private float _attack;
        
        public float Attack
        {
            get
            {
                return _str*1.1f;
            }
        }

        private int _str;
        private Peasant(Dictionary<int, float> LVLTable) :base(2,0,"Peasant",LVLTable)
        {
        }
        public Peasant(int Str, Dictionary<int, float> LVLTable) :this(LVLTable)
        {
            _str = Str;
        }
        public override void MakeDmg(IDmgable dmgable)
        {
            dmgable.GetDamage((int)Attack);
        }

        public void GetDamage(int Amount)
        {
            _health -= Amount;
        }

    }
}
