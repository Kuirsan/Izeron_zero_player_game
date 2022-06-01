using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Interfaces
{
    public interface ILootable
    {
        public void Loot();
        public int Volume { get;}
        public string Name { get; }
    }
}
