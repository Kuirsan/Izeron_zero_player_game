namespace Izeron.Library.Objects
{
    /// <summary>
    /// Base class for others
    /// </summary>
    public abstract class AbstractMainObject
    {
        private protected string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

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
