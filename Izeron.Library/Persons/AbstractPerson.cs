using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.InventorySystem;
using Izeron.Library.Objects;
using Izeron.Library.Objects.LootableObjects;
using Izeron.Library.Objects.Potions;
using System;
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
        private protected InventoryBase _inventory;
        public Func<string, bool> IsQuestItem { get; set; }
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
        virtual public List<LootViewModel> InventoryList
        {
            get
            {
                return _inventory.GetInventoryList();
            }
        }


        virtual public bool IsDead()
        {
            return _isDead;
        }

        protected virtual void Death()
        {
            _isDead = true;
        }

        virtual public bool SomethingInInventory()
        {
            return false;
        }

        public virtual bool AddItemToInventory(ILootable item)
        {
            return false;
        }

        virtual public bool IsAnyMoney()
        {
            return _money > 0;
        }
        abstract public int AttackAmount();
        public virtual void AddMoneyAmount(int money)
        {
            _money += money;
        }
        public virtual void SellItemInInventory()
        {
            return;
        }
        public virtual int Money => _money;
        public virtual void SetMoneyAmount(int value) 
        { 
            return; 
        }

        public virtual void AddHealthPotion(HealthPotionBase healthPotion)
        {
            _inventory.TryToAddHealthPotion(healthPotion);
        }

        public virtual void ConsumeHealthPotion() 
        {
            return;
        }

        public virtual bool HasHealthPotion()
        {
            return _inventory.HasHealthPotions();
        }

        public virtual bool CanAddAnotherHealthPotion()
        {
            return !_inventory.IsFullOfHealthPotions();
        }
    }
}
