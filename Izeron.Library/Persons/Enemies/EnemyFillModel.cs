using Izeron.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Persons.Enemies
{
    public class EnemyFillModel
    {
        public int Attack { get; set; }
        public int HP { get; set; }
        public string Name { get; set; }
        public int XPToGain { get; set; }
        public  HashSet<SpecialEnemyTags> EnemyTags { get; set; }
    }
}
