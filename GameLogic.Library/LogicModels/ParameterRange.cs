using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LogicModels
{
    public class ParameterRange
    {
        public int MinParameter { get; set; }
        public int MaxParameter { get; set; }
        public ParameterRange(int min, int max)
        {
            MinParameter = min;
            MaxParameter = max;
        }
        public ParameterRange()
        {

        }
    }
}
