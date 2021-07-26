using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LogicModels
{
    public class ParameterRange
    {
        public int minParameter { get; set; }
        public int maxParameter { get; set; }
        public ParameterRange(int min, int max)
        {
            minParameter = min;
            maxParameter = max;
        }
        private ParameterRange()
        {

        }
    }
}
