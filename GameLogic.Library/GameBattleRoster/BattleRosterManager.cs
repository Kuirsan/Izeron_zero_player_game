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
                var tmpRoster = GetMonsterRosterForFloor(1);
                tmpRoster.AddRange(monsters);
                _battleRosterByFloor[floor] = tmpRoster;
            }
            else
            {
                _battleRosterByFloor.Add(floor, monsters.ToList());
            }
        }
        public List<AbstractPerson> GetMonsterRosterForFloor(int floor)
        {
            if (_battleRosterByFloor.ContainsKey(floor))
            {
                return _battleRosterByFloor[floor];
            }
            return null;
        }

        public List<AbstractPerson> GenerateRandomMonsters(int floor, int amount)
        {
            List<AbstractPerson> monsters = new List<AbstractPerson>();
            if (_enemiesModels == null) return monsters;
            List<GameEnemiesModel> monsterModels = _enemiesModels
                                                    .Where(x => x.FloorRange.MinParameter <= floor && x.FloorRange.MaxParameter >= floor)
                                                    .Select(x => x).ToList();
            for (int i = 0; i < amount; i++)
            {
                GameEnemiesModel monsterModel=new GameEnemiesModel(null,null,null,null);
                if (monsterModels.Count == 0) return monsters;
                if (monsterModels.Count == 1) monsterModel = monsterModels[0];
                if (monsterModels.Count > 1)
                {
                    monsterModel = monsterModels[new Random().Next(0, monsterModels.Count)];
                }
                monsters.Add(GenerateMonster(monsterModel));
            }
            return monsters;
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
                monsterByFloor.Value.RemoveAll(x => x.IsDead());
            }
            return gameNotification;
        }
    }
}
