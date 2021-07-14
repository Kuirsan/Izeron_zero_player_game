using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using GameCenter.Library.GameCenter;
using GameLogic.Library.GameBattleRoster;
using GameLogic.Library.GameStateLogic;
using Izeron.Library.Enums;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Objects.LootableObjects;
using Izeron.Library.Persons;
using Izeron.Library.Persons.Enemies.Tier0;
using Izeron.Library.Persons.Tier0;
using QuestHandlerSystem.Library;
using QuestHandlerSystem.Library.Quest.Models;

namespace SomeKindOfGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static CancellationTokenSource cancelToken = new CancellationTokenSource();
        AbstractPerson Pers;
        AbstractPerson Enemy;
        someclass battleClass;
        QuestObserver quests;
        GameProcess gameProcess;
        BattleRosterManager monsterRoster=new BattleRosterManager();

        public MainWindow()
        {

            Dictionary<int, float> dict = new Dictionary<int, float>
            {
                {0,11f},{1,20f},{2,30f},{3,40f},{4,50f},{5,60f},{6,70f},{7,80f},{8,90f},{9,100f},{10,130f}
            };
            Pers = new Peasant(1, dict);
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 3));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 3));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 3));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 1));
            Pers.AddItemToInventory(new CommonLoot("Branch", 3));
            Enemy = new Peasant(1, dict);
            quests = GameManager.InitiateQuestObserver(Pers);
            gameProcess = GameManager.InitiateGameProcess(Pers, new GameStateLogicByHero());
            var monstrRoast = new List<AbstractPerson>()
            {
                new Rat(2, 1, "rat",1),
                new Rat(1, 1, "rat",1),
                new Rat(1, 1, "rat",1)
            };
            monsterRoster.AddMonsterToRoster(1, monstrRoast.ToArray());
            monsterRoster.AddMonsterToRoster(1, monsterRoster.generateRandomMonsters(1, 200).ToArray());
            quests.SignOnQuest(new KillQuest("rats problem", "kill 3 rats", monstrRoast, new RewardModel { xpReward = 10,goldReward=15 }));
            battleClass = new someclass(Pers, monsterRoster.getMonsterRoastForFloor(1).ToList());
            InitializeComponent();
            this.timeNow.Content = $"Сейчас: {DateTime.Now.ToShortTimeString()}";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.25);
            timer.Tick += timer_Tick;
            timer.Start();
            LoadGridHero();
            if (Pers is AbstractPersonTier0 pp)
            {
                Binding b = new Binding();
                b.Source = Pers;
                b.Path = new PropertyPath(nameof(pp.CurrentXP));
                b.Mode = BindingMode.OneWay;
                Binding b1 = new Binding();
                b1.Source = Pers;
                b1.Path = new PropertyPath(nameof(pp.MaxXP));
                b1.Mode = BindingMode.OneWay;
                this.expBar.SetBinding(ProgressBar.ValueProperty, b);
                this.expBar.SetBinding(ProgressBar.MaximumProperty, b1);
                Binding b2 = new Binding();
                b2.Source = Pers;
                b2.Path = new PropertyPath(nameof(pp.CharacterList));
                b2.Mode = BindingMode.OneWay;
                this.gridHero.SetBinding(DataGrid.ItemsSourceProperty, b2);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {

            string dateTime = DateTime.Now.ToShortTimeString();
            this.timeNow.Content = $"Сейчас: {dateTime}";
            string allMessage = string.Empty;
            string FightMessage = string.Empty;
            string anotherMessage = string.Empty;
            string QuestMessage = string.Empty;
            if (gameProcess.CurrentState == GameState.Fighting)
            {
                try
                {
                    GameManager.GameTick(new IUpdatable[] { battleClass, quests, monsterRoster });
                }
                catch (YouDeadException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                gameProcess.MoveNext(monsterRoster.getMonsterRoastForFloor(1));
            }
            else if (gameProcess.CurrentState == GameState.BackToTown)
            {
                //anotherMessage = "Возвращаемся в город" + Environment.NewLine;
                gameProcess.MoveNext(null);
            }
            else if(gameProcess.CurrentState==GameState.Healing)
            {
                //anotherMessage = "Лечимся" + Environment.NewLine;
                gameProcess.MoveNext(null);
            }
            else
            {
                GameManager.GameTick(new IUpdatable[] { quests });
                gameProcess.MoveNext(null);
            }
            FightMessage = GameManager.getUnreadLogsString(GameNotificationState.Battle);
            QuestMessage = GameManager.getUnreadLogsString(GameNotificationState.Quest);
            anotherMessage = GameManager.getUnreadLogsString(GameNotificationState.Other);
            allMessage = GameManager.getUnreadLogsString(GameNotificationState.All);

            this.notificationAllText.Text += allMessage;
            this.textAllViewerScroll.ScrollToEnd();

            this.notificationFightText.Text += FightMessage;
            this.textFightViewerScroll.ScrollToEnd();

            this.notificationQuestText.Text += QuestMessage;
            this.textQuestViewerScroll.ScrollToEnd();

            this.notificationAnotherText.Text += anotherMessage;
            this.textAnotherViewerScroll.ScrollToEnd();

        }

        class someclass : IUpdatable
        {
            AbstractPerson Pers;
            List<AbstractPerson> Enemies;

            public someclass(AbstractPerson pers, List<AbstractPerson> enemies)
            {
                this.Pers = pers;
                this.Enemies = enemies;
            }
            public GameNotification Update()
            {
                GameNotification notification = new GameNotification()
                {
                    gameNotificationState = GameNotificationState.Battle
                };
                if (Enemies.Count == 0) return notification;
                var Enemy = Enemies.First();
                if (Enemy is IDmgable dmg)
                {
                    Pers.MakeDmg(dmg);
                    notification.body += @$"Наносим {Pers.attackAmount()} урона по противнику [{Enemy}]!" + Environment.NewLine;
                }
                if(Enemy.isDead())
                {
                    notification.body += @$"Противник [{Enemy}] получает смертельную рану!" + Environment.NewLine;
                    if(Enemy is IXPTransmittable xpTransmittable)
                    {
                        if(Pers is IXPRecievable xpRecievable)
                        {
                            xpTransmittable.TransmitXP(xpRecievable);
                        }
                    }
                }
                else
                {
                    if (Pers is IDmgable pdmg)
                    {
                        Enemy.MakeDmg(pdmg);
                        notification.body += $@"Противник [{Enemy}] наносит {Enemy.attackAmount()} урона по герою!" + Environment.NewLine;
                    }
                }
                Enemies.RemoveAll(x => x.isDead());
                return notification;
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancelToken.Cancel();
        }
        private void LoadGridHero()
        {
            this.gridHero.HeadersVisibility = DataGridHeadersVisibility.None;
            if (Pers is Peasant pst)
            {
                this.gridHero.ItemsSource = pst.CharacterList;
            }

        }

        private void expBar_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if(Pers is AbstractPersonTier0 ap)
            {
                ((ProgressBar)e.Source).ToolTip = $"exp for next level: {ap.MaxXP - ap.CurrentXP} exp.";
            }

        }
    }
}
