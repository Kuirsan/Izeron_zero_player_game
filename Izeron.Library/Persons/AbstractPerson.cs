using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.InventorySystem;
using Izeron.Library.Objects;
using Izeron.Library.Objects.LootableObjects;
using Izeron.Library.Objects.Potions;
using Izeron.Library.Objects.Equipment;
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
        public virtual string ClassName => "Unknown";

        virtual public Dictionary<string, string> CharacterList
        {
            get
            {
                Dictionary<string, string> valPairs = new Dictionary<string, string>();
                valPairs.Add("Класс", this.ClassName);
                valPairs.Add("Уровень", this._lvl.ToString());
                valPairs.Add("Здоровье", $"{this._health}\\{this._maxHealth}");
                valPairs.Add("Тэги", string.Join("; ", _personTags));
                
                foreach (var item in _equipment)
                {
                    valPairs.Add(item.Key.ToString(), item.Value.Name);
                }
                
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
        // Equipment
        private protected Dictionary<EquipmentSlot, EquipmentBase> _equipment = new Dictionary<EquipmentSlot, EquipmentBase>();

        public virtual void Equip(EquipmentBase item)
        {
            if (item == null) return;
            // Un-equip current item in slot if exists
            if (_equipment.ContainsKey(item.Slot))
            {
                Unequip(item.Slot);
            }
            _equipment[item.Slot] = item;
            // Remove from inventory? Handled by caller usually, but logic dictates equipment is "on body".
        }

        public virtual void Unequip(EquipmentSlot slot)
        {
            if (_equipment.ContainsKey(slot))
            {
                var item = _equipment[slot];
                _equipment.Remove(slot);
                AddItemToInventory(item);
            }
        }

        public int GetTotalEquipmentAttack()
        {
            int total = 0;
            foreach (var item in _equipment.Values)
            {
                total += item.AttackBonus;
            }
            return total;
        }

        public int GetTotalEquipmentDefense()
        {
            int total = 0;
            foreach (var item in _equipment.Values)
            {
                total += item.DefenseBonus;
            }
            return total;
        }

        public Dictionary<EquipmentSlot, EquipmentBase> GetEquipment()
        {
            return _equipment;
        }
    }
}
