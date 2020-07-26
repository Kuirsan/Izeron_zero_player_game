using Izeron.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.InventorySystem
{
    public abstract class InventoryBase
    {
        private protected int _cellNumber;
        private protected List<ILootable> _inventory;
        public abstract void Add(ILootable item);
        public abstract void Remove(ILootable item);
    }
}
