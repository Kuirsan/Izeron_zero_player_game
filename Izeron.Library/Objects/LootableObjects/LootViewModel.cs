﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Objects.LootableObjects
{
    public class LootViewModel
    {
        public LootViewModel(LootableBaseObject lootableBaseObject)
        {
            Name = lootableBaseObject.Name;
            Cost = lootableBaseObject.Volume;
            qty = 1;
        }

        public string Name { get; set; }
        public int Cost { get; set; }
        public int qty { get; set; }
    }
}
