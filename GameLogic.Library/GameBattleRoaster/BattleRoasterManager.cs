using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Library.GameBattleRoster
{
    public class BattleRosterManager:IUpdatable
    {
        private Dictionary<int, List<AbstractPerson>> _battleRosterByFloor;

        public BattleRosterManager()
        {
            Init();
        }
        private void Init()
        {
            _battleRosterByFloor = new Dictionary<int, List<AbstractPerson>>();
        }

        public void AddMonsterToRoster(int floor,AbstractPerson[] monsters)
        {
            if (_battleRosterByFloor.ContainsKey(floor))
            {
                var tmpRoster = getMonsterRoastForFloor(1);
                tmpRoster.AddRange(monsters);
                _battleRosterByFloor[floor] = tmpRoster;
            }
            else
            {
                _battleRosterByFloor.Add(floor, monsters.ToList());
            }
        }
        public List<AbstractPerson> getMonsterRoastForFloor(int floor)
        {
            if (_battleRosterByFloor.ContainsKey(floor))
            {
                return _battleRosterByFloor[floor];
            }
            return null;
        }

        public GameNotification Update()
        {
            GameNotification gameNotification = new GameNotification
            {
                gameNotificationState = Izeron.Library.Enums.GameNotificationState.Battle
            };
            foreach(var monsterByFloor in _battleRosterByFloor)
            {
                monsterByFloor.Value.RemoveAll(x => x.isDead());
            }
            return gameNotification;
        }
    }
}
