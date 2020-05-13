namespace Izeron.Library.Objects
{
    /// <summary>
    /// Base class for others
    /// </summary>
    public abstract class AbstractMainObject
    {
        private readonly protected string _name;
        private protected AbstractMainObject(string Name)
        {
            _name = Name;
        }
        public override string ToString()
        {
            return _name;
        }
    }
}
