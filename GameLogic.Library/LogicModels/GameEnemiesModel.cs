using Izeron.Library.Enums;
using Izeron.Library.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Library.LogicModels
{
    public class GameEnemiesModel
    {
        public string Name { get; set; } = "defaultName";
        public ParameterRange AttackRange { get; set; } = new ParameterRange(1, 1);
        public int XPToGain { get; set; } = 0;
        public SpecialEnemyTag[] possibleTags { get; set; }
        public ParameterRange HPRange { get; set; }
        public ParameterRange FloorRange { get; set; }
        private GameEnemiesModel()
        {

        }
        public GameEnemiesModel(SpecialEnemyTag[] possibleTags, ParameterRange HPRange,ParameterRange FloorRange)
        {
            this.possibleTags = possibleTags;
            this.HPRange = HPRange;
            this.FloorRange = FloorRange;
        }
    }

    public class ParameterRange
    {
        public int minParameter { get; set; }
        public int maxParameter { get; set; }
        public ParameterRange(int min,int max)
        {
            minParameter = min;
            maxParameter = max;
        }
        private ParameterRange()
        {

        }
    }
    public class SpecialEnemyTag
    {
        public SpecialEnemyTags specialEnemyTag { get; set; }
        public string SpecialTagString => specialEnemyTag.ToString();
        public SpecialEnemyTag(SpecialEnemyTags tag)
        {
            specialEnemyTag = tag;
        }
        private SpecialEnemyTag()
        {

        }
    }
}
