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
        private static string _excMessage = string.Empty;
        private static QuestObserver _questObserver;
        private static GameProcess _gameProcess;
        private static List<GameNotification> _gameLogs= new List<GameNotification>();

        private GameManager() { }

        public static GameManager getInstance()
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
                            body = notification.body,
                            gameNotificationState = GameNotificationState.All,
                            isRead = false
                        });
                    }
                }
            }
            catch (YouDeadException ex) 
            {
                _lose = true;
                _excMessage = ex.Message;
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                _isRunTick = false;
                updateLogs();
            }
        }

        private static void updateLogs()
        {
            _gameLogs.RemoveAll(x => string.IsNullOrEmpty(x.body));
            var keys = _gameLogs.GroupBy(x => x.gameNotificationState).Where(x => x.Count() > 10).Select(x => x.Key).ToList();
            foreach(var key in keys)
            {
                _gameLogs.Remove(_gameLogs.Where(x => x.gameNotificationState == key).FirstOrDefault());
            }
        }

        public static List<GameNotification> getUnreadLogs(GameNotificationState gameNotificationState)
        {
            var notification = _gameLogs.Where(log => log.gameNotificationState == gameNotificationState && log.isRead == false).Select(log => log);
            foreach(var not in notification)
            {
                not.isRead = true;
            }
            return notification.ToList();
        }

        public static string getUnreadLogsString(GameNotificationState gameNotificationState)
        {
            var notification = _gameLogs.Where(log => log.gameNotificationState == gameNotificationState && log.isRead == false).Select(log => log);
            StringBuilder sb = new StringBuilder();
            foreach (var not in notification)
            {
                not.isRead = true;
                sb.Append($"[{not.TimeStamp}] {not.body}");
            }
            return sb.ToString();
        }

        public static QuestObserver InitiateQuestObserver(AbstractPerson hero)
        {
            if(_questObserver==null)
            {
                _questObserver = new QuestObserver(hero);
                return _questObserver;
            }
            return _questObserver;
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
