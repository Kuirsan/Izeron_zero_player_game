using Izeron.Library.Enums;

namespace GameLogic.Library.LogicModels
{
    public class GameEnemiesModel
    {
        public string Name { get; set; } = "defaultName";
        public ParameterRange AttackRange { get; set; } = new ParameterRange(1, 1);
        public int XPToGain { get; set; } = 0;
        public SpecialEnemyTag[] PossibleTags { get; set; }
        public EnemyTypeTag[] EnemyTypeTags { get; set; }
        public ParameterRange HPRange { get; set; }
        public ParameterRange FloorRange { get; set; }
        private GameEnemiesModel()
        {

        }
        public GameEnemiesModel(SpecialEnemyTag[] PossibleTags, ParameterRange HPRange,ParameterRange FloorRange, EnemyTypeTag[] enemyTypetags)
        {
            this.PossibleTags = PossibleTags;
            this.HPRange = HPRange;
            this.FloorRange = FloorRange;
            this.EnemyTypeTags = enemyTypetags;
        }
    }

    public class SpecialEnemyTag
    {
        public SpecialEnemyTags TypeOfSpecialEnemyTag { get; set; }
        public string SpecialTagString => TypeOfSpecialEnemyTag.ToString();
        public SpecialEnemyTag(SpecialEnemyTags tag)
        {
            TypeOfSpecialEnemyTag = tag;
        }
        public SpecialEnemyTag()
        {

        }
    }

    public class EnemyTypeTag
    {
        public PersonTags TypeOfEnemyTypeTag { get; set; }
        public string EnemyTypeTagString => TypeOfEnemyTypeTag.ToString();
        public EnemyTypeTag(PersonTags tag)
        {
            TypeOfEnemyTypeTag = tag;
        }
        public EnemyTypeTag()
        {

        }
    }
}
