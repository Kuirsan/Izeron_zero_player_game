using GameLogic.Library.LogicModels;
using Izeron.Library.Enums;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using Izeron.Library.Persons.Enemies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GameLogic.Library.GameBattleRoster
{
    public class BattleRosterManager : IUpdatable
    {
        private Dictionary<int, List<AbstractPerson>> _battleRosterByFloor;
        private List<GameEnemiesModel> _enemiesModels;

        public delegate void EnemiesLootHandle(int floorNumber, int enemiesNumber);
        public event EnemiesLootHandle NotifyLootSystem;
        
        public delegate void MonsterKilledHandle(int count);
        public event MonsterKilledHandle OnMonstersKilled;

        public BattleRosterManager()
        {
            Init();
        }
        private void Init()
        {
            _battleRosterByFloor = new Dictionary<int, List<AbstractPerson>>();
            LoadEnemiesModels();
        }

        private void LoadEnemiesModels()
        {
            string fileName = @"EnemiesLibrary\enemies.json";
            _enemiesModels = GetEnemyModelsFromJSON(fileName);
        }

        private List<GameEnemiesModel> GetEnemyModelsFromJSON(string path)
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<GameEnemiesModel>>(jsonString,new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public void AddMonsterToRoster(int floor, AbstractPerson[] monsters)
        {
            if (_battleRosterByFloor.ContainsKey(floor))
            {
                var tmpRoster = GetMonsterRoastForFloor(1);
                tmpRoster.AddRange(monsters);
                _battleRosterByFloor[floor] = tmpRoster;
            }
            else
            {
                _battleRosterByFloor.Add(floor, monsters.ToList());
            }
        }
        public List<AbstractPerson> GetMonsterRoastForFloor(int floor)
        {
            if (_battleRosterByFloor.ContainsKey(floor))
            {
                return _battleRosterByFloor[floor];
            }
            return null;
        }

        public List<AbstractPerson> GenerateRandomMonsters(int floor, int amount)
        {
            return GenerateRandomMonstersByPower(floor, amount, 0);
        }

        public List<AbstractPerson> GenerateRandomMonstersByPower(int floor, int amount, int heroPowerRating)
        {
            List<AbstractPerson> monsters = new List<AbstractPerson>();
            if (_enemiesModels == null) return monsters;
            
            // Фильтруем врагов по этажу
            List<GameEnemiesModel> monsterModels = _enemiesModels
                .Where(x => x.FloorRange.MinParameter <= floor && x.FloorRange.MaxParameter >= floor)
                .ToList();

            if (monsterModels.Count == 0) return monsters;

            for (int i = 0; i < amount; i++)
            {
                GameEnemiesModel monsterModel = SelectMonsterByPower(monsterModels, heroPowerRating);
                monsters.Add(GenerateMonster(monsterModel));
            }
            return monsters;
        }

        private GameEnemiesModel SelectMonsterByPower(List<GameEnemiesModel> availableMonsters, int heroPowerRating)
        {
            if (availableMonsters.Count == 1) return availableMonsters[0];

            // Создаем взвешенный список на основе силы героя
            var weightedMonsters = new List<GameEnemiesModel>();
            
            foreach (var monster in availableMonsters)
            {
                // Рассчитываем вес на основе разницы между силой героя и врага
                int powerDiff = heroPowerRating - monster.PowerRating;
                int weight;

                // Если враг слишком силен для героя, исключаем его полностью
                if (powerDiff < -15)
                {
                    // Враг на 15+ пунктов сильнее - не появляется совсем
                    continue;
                }
                else if (powerDiff < -10)
                {
                    // Враг на 10-15 пунктов сильнее - очень редко
                    weight = 1;
                }
                else if (powerDiff >= 10)
                {
                    // Герой намного сильнее - низкий шанс слабого врага
                    weight = 2;
                }
                else if (powerDiff >= 5)
                {
                    // Герой сильнее - средний шанс
                    weight = 3;
                }
                else if (powerDiff >= 0)
                {
                    // Примерно равны - высокий шанс
                    weight = 10;
                }
                else if (powerDiff >= -5)
                {
                    // Враг немного сильнее - очень высокий шанс (челлендж)
                    weight = 8;
                }
                else // powerDiff >= -10
                {
                    // Враг значительно сильнее - средний шанс
                    weight = 3;
                }

                for (int i = 0; i < weight; i++)
                {
                    weightedMonsters.Add(monster);
                }
            }

            // Если после фильтрации не осталось монстров, берем самого слабого
            if (weightedMonsters.Count == 0)
            {
                var weakestMonster = availableMonsters.OrderBy(m => m.PowerRating).First();
                return weakestMonster;
            }

            return weightedMonsters[new Random().Next(weightedMonsters.Count)];
        }

        public List<AbstractPerson> GenerateMonstersByName(string[] monstersName)
        {
            List<AbstractPerson> monsters = new List<AbstractPerson>();
            if (_enemiesModels == null) return monsters;

            foreach(var mName in monstersName)
            {
                GameEnemiesModel monsterModel = _enemiesModels.Where(m => m.Name.ToLower() == mName.ToLower()).FirstOrDefault();
                if (monsterModel == null) continue;
                monsters.Add(GenerateMonster(monsterModel));
            }

            return monsters;
        }

        private AbstractPerson GenerateMonster(GameEnemiesModel monsterModel)
        {
            HashSet<SpecialEnemyTags> enemyTags = new HashSet<SpecialEnemyTags>();
            foreach(var tag in monsterModel.PossibleTags)
            {
                if (new Random().Next(100) > 70) enemyTags.Add(tag.TypeOfSpecialEnemyTag);
            }
            EnemyFillModel enemyFillModel = new EnemyFillModel
            {
                Attack = new Random().Next(monsterModel.AttackRange.MinParameter, monsterModel.AttackRange.MaxParameter),
                HP = new Random().Next(monsterModel.HPRange.MinParameter, monsterModel.HPRange.MaxParameter),
                Name = monsterModel.Name,
                XPToGain = monsterModel.XPToGain,
                EnemyTags=enemyTags
            };

            return new Monster(enemyFillModel);
        }

        public GameNotification Update()
        {
            GameNotification gameNotification = new GameNotification
            {
                GameNotificationState = GameNotificationState.Battle
            };
            foreach (var monsterByFloor in _battleRosterByFloor)
            {
                int floor = monsterByFloor.Key;
                int countDeadMonsters = monsterByFloor.Value.Where(x => x.IsDead()).Count();
                NotifyLootSystem?.Invoke(floor, countDeadMonsters);
                if (countDeadMonsters > 0)
                {
                    OnMonstersKilled?.Invoke(countDeadMonsters);
                }
                monsterByFloor.Value.RemoveAll(x => x.IsDead());
            }
            return gameNotification;
        }
    }
}
