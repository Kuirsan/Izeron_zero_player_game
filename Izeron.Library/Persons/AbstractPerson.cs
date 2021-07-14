using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.InventorySystem;
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
        private protected bool _isDead = false;
        private protected int _money;
        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
        }
        public int CurrentHealth
        {
            get
            {
                return _health;
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
        virtual public bool isDead()
        {
            return _isDead;
        }

        protected virtual void Death()
        {
            _isDead = true;
        }

        virtual public bool somethingInInventory()
        {
            return false;
        }

        public virtual bool AddItemToInventory(ILootable item)
        {
            return false;
        }

        virtual public bool isAnyMoney()
        {
            return _money > 0;
        }
        abstract public int attackAmount();
        public virtual void addMoneyAmount(int money)
        {
            _money += money;
        }
        public virtual void sellIteminInventory()
        {

        }
        public virtual int Money => _money;
        public virtual void setMoneyAmount(int value) { }
    }
}
