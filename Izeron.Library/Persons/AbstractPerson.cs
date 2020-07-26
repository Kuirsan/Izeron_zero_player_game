using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Objects;
using System.Collections.Generic;

namespace Izeron.Library.Persons
{
    /// <summary>
    /// Base class for another persons
    /// </summary>
    public abstract class AbstractPerson : AbstractMainObject
    {
        private protected int _health;
        private protected int _maxHealth;
        private protected int _lvl;
        private protected HashSet<PersonTags> _personTags;
        public int Health
        {
            get
            {
                return _maxHealth;
            }
        }
        private protected AbstractPerson(int HP, int LVL, string Name) : base(Name)
        {
            _maxHealth = HP;
            _health = _maxHealth;
            _lvl = LVL;
            _personTags = new HashSet<PersonTags>();
        }
        /// <summary>
        /// Hit only those who implement IDmgable
        /// </summary>
        /// <param name="dmgable">Instance of interface</param>
        abstract public void MakeDmg(IDmgable dmgable);
        virtual public Dictionary<string, string> CharacterList
        {
            get
            {
                Dictionary<string, string> valPairs = new Dictionary<string, string>();
                valPairs.Add("Класс", this.ToString());
                valPairs.Add("Уровень", this._lvl.ToString());
                valPairs.Add("Здоровье", $"{this._health}\\{this._maxHealth}");
                valPairs.Add("Тэги", string.Join("; ", _personTags));
                return valPairs;
            }
        }

    }
}
