using GameLogic.Library.GameBattleRoster;
using GameLogic.Library.GameStateLogic;
using Izeron.Library.Enums;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Persons;
using QuestHandlerSystem.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCenter.Library.GameCenter
{
    public class GameManager
    {
        private static GameManager _instance;
        private static object _syncRoot = new object();
        private static bool _isRunTick = false;
        private static bool _lose = false;
        private static QuestObserver _questObserver;
        private static GameProcess _gameProcess;
        private static List<GameNotification> _gameLogs= new List<GameNotification>();

        private GameManager() { }

        public static GameManager GetInstance()
        {
            if (_instance == null)
            {
                lock(_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new GameManager();
                    }
                }
            }
            return _instance;
        }
        
        public static void GameTick(IUpdatable[] updatableObjects)
        {
            if (_lose) return;
            try
            {
                if (_isRunTick) return;
                lock (_syncRoot)
                {
                    if (_isRunTick) return;
                    _isRunTick = true;
                    foreach (var obj in updatableObjects)
                    {
                        GameNotification notification = obj.Update();
                        _gameLogs.Add(notification);
                        _gameLogs.Add(new GameNotification
                        {
                            Body = notification.Body,
                            GameNotificationState = GameNotificationState.All,
                            IsRead = false
                        });
                    }
                }
            }
            catch (YouDeadException ex) 
            {
                _lose = true;
                throw ex;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _isRunTick = false;
                UpdateLogs();
            }
        }

        private static void UpdateLogs()
        {
            _gameLogs.RemoveAll(x => string.IsNullOrEmpty(x.Body));
            var keys = _gameLogs.GroupBy(x => x.GameNotificationState).Where(x => x.Count() > 10).Select(x => x.Key).ToList();
            foreach(var key in keys)
            {
                var toRemove = _gameLogs.Where(x => x.GameNotificationState == key).FirstOrDefault();
                if (toRemove != null)
                {
                    _gameLogs.Remove(toRemove);
                }
            }
        }

        public static List<GameNotification> GetUnreadLogs(GameNotificationState gameNotificationState)
        {
            var notification = _gameLogs.Where(log => log.GameNotificationState == gameNotificationState && log.IsRead == false).ToList();
            foreach(var not in notification)
            {
                not.IsRead = true;
            }
            return notification;
        }

        public static string GetUnreadLogsString(GameNotificationState gameNotificationState)
        {
            var notification = _gameLogs.Where(log => log.GameNotificationState == gameNotificationState && log.IsRead == false).Select(log => log);
            StringBuilder sb = new StringBuilder();
            foreach (var not in notification)
            {
                not.IsRead = true;
                sb.Append($"[{not.TimeStamp}] {not.Body}");
            }
            return sb.ToString();
        }

        public static QuestObserver InitiateQuestObserver(AbstractPerson hero, BattleRosterManager monsterManager, Func<int> getCurrentFloor = null)
        {
            if(_questObserver==null)
            {
                _questObserver = new QuestObserver(hero, monsterManager, getCurrentFloor);
                return _questObserver;
            }
            return _questObserver;
        }
        
        public static void AddNotification(GameNotification notification)
        {
            _gameLogs.Add(notification);
            _gameLogs.Add(new GameNotification
            {
                Body = notification.Body,
                GameNotificationState = GameNotificationState.All,
                IsRead = false
            });
        }
        
        public static GameProcess InitiateGameProcess(AbstractPerson hero, BaseGameStateLogic logic)
        {
            if(_gameProcess==null)
            {
                _gameProcess = new GameProcess(hero, logic);
            }
            return _gameProcess;
        }
    }
}
