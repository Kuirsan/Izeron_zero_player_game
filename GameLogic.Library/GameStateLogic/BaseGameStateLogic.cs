using Izeron.Library.Enums;
using Izeron.Library.Persons;

namespace GameLogic.Library.GameStateLogic
{
    public abstract class BaseGameStateLogic
    {
        public abstract GameState GetNextGameStateByPerson(AbstractPerson person,GameState currentState,object opt);
    }
}
