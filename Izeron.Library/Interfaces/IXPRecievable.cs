namespace Izeron.Library.Interfaces
{
    /// <summary>
    /// Interface to markup those classes that can receive expirience
    /// </summary>
    public interface IXPRecievable
    {
        /// <summary>
        /// Get amount of xp
        /// </summary>
        /// <param name="Amount"></param>
        public void ReceiveXP(int Amount);
    }
}
