using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Objects.Potions;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.GameStateLogic
{
    public class GameStateLogicByHero : BaseGameStateLogic
    {
        public override GameState GetNextGameStateByPerson(AbstractPerson person, GameState currentState, object opt)
        {
            if (currentState == GameState.InTown) return InTownState(person);
            if (currentState == GameState.SellingLoot) return SellingLootState(person);
            if (currentState == GameState.BuyGears) return BuyingGearState(person);
            if (currentState == GameState.Healing) return HealingState(person);
            if (currentState == GameState.Explorirng) return GameState.Fighting;
            if (currentState == GameState.Fighting) return FightingState(person, opt);
            if (currentState == GameState.Looting) return LootingState(person, opt);
            if (currentState == GameState.BackToTown) return GameState.InTown;
            return currentState;
        }
        private static GameState InTownState(AbstractPerson hero)
        {
            if (hero.SomethingInInventory()) return GameState.SellingLoot;

            if (hero.CurrentHealth < hero.MaxHealth) return GameState.Healing;

            if (hero.IsAnyMoney()) return GameState.BuyGears;

            return GameState.Explorirng;
        }
        private static GameState FightingState(AbstractPerson hero, object opt)
        {
            if (opt is IList<AbstractPerson> enemies)
            {
                if (enemies.Count == 0) return GameState.Looting;
            }
            if (hero.CurrentHealth <= (hero.MaxHealth * 0.3))
            {
                if (hero.HasHealthPotion())
                {
                    hero.ConsumeHealthPotion();
                    return GameState.Fighting;
                }
                else
                {
                    return GameState.Looting;
                }
            }
            return GameState.Fighting;
        }

        private static GameState LootingState(AbstractPerson hero, object opt)
        {
            if (opt is ILootable loot)
            {
                hero.AddItemToInventory(loot);
                return GameState.Looting;
            }
            return GameState.BackToTown;
        }

        private static GameState HealingState(AbstractPerson hero)
        {
            if (hero.CurrentHealth == hero.MaxHealth) return GameState.InTown;
            if (hero is IHealable healHero)
            {
                healHero.GetHeal(1 + (int)(hero.MaxHealth * 0.1));
            }
            return GameState.Healing;
        }
        private static GameState SellingLootState(AbstractPerson hero)
        {
            if (!hero.SomethingInInventory()) return GameState.InTown;
            hero.SellItemInInventory();
            return GameState.SellingLoot;

        }
        private GameState BuyingGearState(AbstractPerson hero)
        {
            HealthPotionBase potion = new SmallHealthPotion();

            hero.AddMoneyAmount(_bank);
            _bank = 0;

            if (potion.CanBuy(hero) && hero.CanAddAnotherHealthPotion())
            {
                potion.Buy(hero);
                return GameState.BuyGears;
            }
            else
            {
                _bank += hero.Money;
                hero.SetMoneyAmount(0);
                return GameState.InTown;
            }
        }
    }
}
