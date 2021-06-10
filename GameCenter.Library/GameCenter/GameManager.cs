using GameLogic.Library.GameStateLogic;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using QuestHandlerSystem.Library;
using System;
using System.Collections.Generic;
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
        
        public static string GameTick(IUpdatable[] updatableObjects)
        {
            string notification = string.Empty;
            if (_lose) return string.Empty;
            try
            {
                if (_isRunTick) return string.Empty;
                lock (_syncRoot)
                {
                    if (_isRunTick) return string.Empty;
                    _isRunTick = true;
                    foreach (var obj in updatableObjects)
                    {
                        notification+= obj.Update()+Environment.NewLine;
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
            }
            return notification;
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
