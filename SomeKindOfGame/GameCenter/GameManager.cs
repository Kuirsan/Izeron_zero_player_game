using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using QuesHandlerSystem.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace SomeKindOfGame.GameCenter
{
    public class GameManager
    {
        private static GameManager _instance;
        private static object _syncRoot = new object();
        private static bool _isRunTick = false;
        private static bool _lose = false;
        private static string _excMessage = string.Empty;
        private static QuestObserver _questObserver;

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
                        obj.Update();
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
    }
}
