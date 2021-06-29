using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Library.GameBattleRoaster
{
    public class BattleRoasterManager:IUpdatable
    {
        private Dictionary<int, List<AbstractPerson>> _battleRoasterByFloor;

        public BattleRoasterManager()
        {
            Init();
        }
        private void Init()
        {
            _battleRoasterByFloor = new Dictionary<int, List<AbstractPerson>>();
        }

        public void AddMonsterToRoaset(int floor,AbstractPerson[] monsters)
        {
            if (_battleRoasterByFloor.ContainsKey(floor))
            {
                var tmpRoaster = getMonsterRoastForFloor(1);
                tmpRoaster.AddRange(monsters);
                _battleRoasterByFloor[floor] = tmpRoaster;
            }
            else
            {
                _battleRoasterByFloor.Add(floor, monsters.ToList());
            }
        }
        public List<AbstractPerson> getMonsterRoastForFloor(int floor)
        {
            if (_battleRoasterByFloor.ContainsKey(floor))
            {
                return _battleRoasterByFloor[floor];
            }
            return null;
        }

        public GameNotification Update()
        {
            GameNotification gameNotification = new GameNotification
            {
                gameNotificationState = Izeron.Library.Enums.GameNotificationState.Battle
            };
            foreach(var monsterByFloor in _battleRoasterByFloor)
            {
                monsterByFloor.Value.RemoveAll(x => x.isDead());
            }
            return gameNotification;
        }
    }
}
