using Izeron.Library.Persons;

namespace GameLogic.Library.FloorSystem
{
    public class FloorManager
    {
        private int _currentFloor = 1;
        private int _monstersKilledOnFloor = 0;
        private const int MonstersPerFloor = 15; // Количество монстров для перехода на следующий этаж

        public delegate void FloorAdvancedHandler(int newFloor);
        public event FloorAdvancedHandler OnFloorAdvanced;

        public int CurrentFloor => _currentFloor;
        public int MonstersKilledOnFloor => _monstersKilledOnFloor;
        public int MonstersToNextFloor => MonstersPerFloor - _monstersKilledOnFloor;

        public void RegisterMonsterKill()
        {
            _monstersKilledOnFloor++;
            
            if (_monstersKilledOnFloor >= MonstersPerFloor)
            {
                AdvanceToNextFloor();
            }
        }

        private void AdvanceToNextFloor()
        {
            _currentFloor++;
            _monstersKilledOnFloor = 0;
            
            // Уведомляем подписчиков о переходе на новый этаж
            OnFloorAdvanced?.Invoke(_currentFloor);
        }

        public void Reset()
        {
            _currentFloor = 1;
            _monstersKilledOnFloor = 0;
        }

        public string GetFloorInfo()
        {
            return $"Этаж {_currentFloor} ({_monstersKilledOnFloor}/{MonstersPerFloor})";
        }
    }
}
