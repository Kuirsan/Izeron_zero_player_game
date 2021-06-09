﻿using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.GameStateLogic
{
    public class GameStateLogicByHero : BaseGameStateLogic
    {
        public override GameState GetNextGameStateByPerson(AbstractPerson person, GameState currentState,object opt)
        {
            if (currentState == GameState.InTown) return InTownState(person);
            if (currentState == GameState.SellingLoot) return GameState.InTown;
            if (currentState == GameState.BuyGears) return GameState.InTown;
            if (currentState == GameState.Healing) return HealingState(person);
            if (currentState == GameState.Explorirng) return GameState.Fighting;
            if (currentState == GameState.Fighting) return FightingState(person,opt);
            if (currentState == GameState.Looting) return GameState.BackToTown;
            if (currentState == GameState.BackToTown) return GameState.InTown;
            return currentState;
        }
        private GameState InTownState(AbstractPerson hero)
        {
            if (hero.somethingInInventory()) return GameState.SellingLoot;

            if (hero.CurrentHealth < hero.MaxHealth) return GameState.Healing;

            if (hero.isAnyMoney()) return GameState.BuyGears;

            return GameState.Explorirng;
        }
        private GameState FightingState(AbstractPerson hero,object opt)
        {
            if(opt is IList<AbstractPerson> enemies)
            {
                if (enemies.Count == 0) return GameState.Looting;
            }
            if (hero.CurrentHealth <= (hero.MaxHealth * 0.3)) return GameState.BackToTown;
            return GameState.Fighting;
        }

        private GameState HealingState(AbstractPerson hero)
        {
            if (hero.CurrentHealth == hero.MaxHealth) return GameState.InTown;
            if(hero is IHealable healHero)
            {
                healHero.getHeal(1 + (int)(hero.MaxHealth * 0.1));
            }
            return GameState.Healing;
        }
    }
}
