namespace Izeron.Library.Interfaces
{
    /// <summary>
    /// Interface to markup those classes that can receive damage
    /// </summary>
    public interface IDmgable
    {
        /// <summary>
        /// Get amount of damage
        /// </summary>
        /// <param name="Amount"></param>
        void GetDamage(int Amount);
    }
}
