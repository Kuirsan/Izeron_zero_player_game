using GameLogic.Library.GameStateLogic;
using Izeron.Library.Enums;
using Izeron.Library.Persons;
using System;

namespace GameCenter.Library.GameCenter
{
    public class GameProcess
    {
        public GameState CurrentState { get; private set; }
        private AbstractPerson _hero;
        private BaseGameStateLogic _gameStateLogic;

        public GameProcess(AbstractPerson Hero,BaseGameStateLogic gameStateLogic)
        {
            CurrentState = GameState.InTown;
            if (Hero == null) throw new NullReferenceException("field Hero is empty");
            if (gameStateLogic == null) throw new NullReferenceException("field gmaeStateLogic is empty");
            _hero = Hero;
            _gameStateLogic = gameStateLogic;
        }
        public GameState MoveNext(object opt)
        {
            CurrentState = _gameStateLogic.GetNextGameStateByPerson(_hero, CurrentState, opt);
            return CurrentState;
        }

    }
}
