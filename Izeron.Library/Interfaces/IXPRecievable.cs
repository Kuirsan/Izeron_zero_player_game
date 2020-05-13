namespace Izeron.Library.Interfaces
{
    /// <summary>
    /// Interface for xp and lvl Logic
    /// </summary>
    public interface IXPRecievable
    {
        /// <summary>
        /// Interface to markup those classes that can receive damage
        /// </summary>
        /// <param name="Amount"></param>
        public void ReceiveXP(float Amount);
    }
}
