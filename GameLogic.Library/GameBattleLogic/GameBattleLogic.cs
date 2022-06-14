using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Library.GameBattleLogic
{
    public class GameBattleLogic : IUpdatable
    {
        private AbstractPerson _pers;
        private List<AbstractPerson> _enemies;

        public GameBattleLogic(AbstractPerson pers, List<AbstractPerson> enemies)
        {
            _pers = pers;
            _enemies = enemies;
        }
        public void SetMonsters(List<AbstractPerson> enemies)
        {
            _enemies = enemies;
        }
        public GameNotification Update()
        {
            GameNotification notification = new GameNotification()
            {
                GameNotificationState = GameNotificationState.Battle
            };
            if (_enemies.Count == 0) return notification;
            var Enemy = _enemies.First();
            if (Enemy is IDmgable dmg)
            {
                _pers.MakeDmg(dmg);
                notification.Body += @$"Наносим {_pers.AttackAmount()} урона по противнику [{Enemy}]!" + Environment.NewLine;
            }
            if (Enemy.IsDead())
            {
                notification.Body += @$"Противник [{Enemy}] получает смертельную рану!" + Environment.NewLine;
                if (Enemy is IXPTransmittable xpTransmittable)
                {
                    if (_pers is IXPRecievable xpRecievable)
                    {
                        xpTransmittable.TransmitXP(xpRecievable);
                    }
                }
            }
            else
            {
                if (_pers is IDmgable pdmg)
                {
                    Enemy.MakeDmg(pdmg);
                    notification.Body += $@"Противник [{Enemy}] наносит {Enemy.AttackAmount()} урона по герою!" + Environment.NewLine;
                }
            }
            _enemies.RemoveAll(x => x.IsDead());
            return notification;
        }
    }
}
