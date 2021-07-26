using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LogicModels
{
    public class GameLootModel
    {
        public string Name { get; set; } = "defaultName";

        public ParameterRange CostInGoldRange {get;set;}

        public ParameterRange FloorRange { get; set; }
    }
}
