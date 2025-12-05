using Izeron.Library.Persons;
using Izeron.Library.Persons.Tier0;

namespace GameLogic.Library.GameBattleRoster
{
    public static class HeroPowerCalculator
    {
        /// <summary>
        /// Рассчитывает рейтинг силы героя на основе его характеристик
        /// </summary>
        public static int CalculatePowerRating(AbstractPerson hero)
        {
            int powerRating = 0;

            // Базовый рейтинг от уровня (каждый уровень = 2 очка силы)
            if (hero is AbstractPersonTier0 heroTier0)
            {
                powerRating += heroTier0.CurrentXP / 10; // Опыт тоже учитывается
            }

            // Здоровье (каждые 5 HP = 1 очко силы)
            powerRating += hero.MaxHealth / 5;

            // Урон (каждая единица урона = 2 очка силы)
            powerRating += hero.AttackAmount() * 2;

            // Бонус от перков (защита и дополнительный урон)
            if (hero is AbstractPersonTier0 heroWithPerks)
            {
                powerRating += heroWithPerks.GetDamageBonus() * 2;
                powerRating += heroWithPerks.GetDefenseBonus() * 3; // Защита ценнее
            }

            return powerRating;
        }
    }
}
