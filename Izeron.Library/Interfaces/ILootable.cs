using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Interfaces
{
    public interface ILootable
    {
        void Loot();
        int Volume { get;}
        string Name { get; }
    }
}
