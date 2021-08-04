using Izeron.Library.Persons;

namespace Izeron.Library.Interfaces
{
    public interface IBuyable
    {
        public int Cost { get; }
        public void Buy(AbstractPerson person);
    }
}
