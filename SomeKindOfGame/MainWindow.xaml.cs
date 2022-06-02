using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using GameCenter.Library.GameCenter;
using GameLogic.Library.GameBattleRoster;
using GameLogic.Library.GameStateLogic;
using GameLogic.Library.LootManager;
using Izeron.Library.Enums;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
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
        someclass battleClass;
        QuestObserver quests;
        GameProcess gameProcess;
        BattleRosterManager monsterRoster = new BattleRosterManager();
        LootManager lootManager = new LootManager();

        public MainWindow()
        {
            InitializeComponent();

            monsterRoster.NotifyLootSystem += lootManager.GenerateLootAndAddByFloor;

            Dictionary<int, float> dict = new Dictionary<int, float>
            {
                {0,11f},{1,20f},{2,30f},{3,40f},{4,50f},{5,60f},{6,70f},{7,80f},{8,90f},{9,100f},{10,130f}
            };
            Pers = new Peasant(1, dict);

            quests = GameManager.InitiateQuestObserver(Pers,monsterRoster);
            gameProcess = GameManager.InitiateGameProcess(Pers, new GameStateLogicByHero());
            var monstrRoster = new List<AbstractPerson>()
            {
                new Rat(2, 1, "rat",1),
                new Rat(1, 1, "rat",1),
                new Rat(1, 1, "rat",1)
            };
            var monstrRoster2 = new List<AbstractPerson>()
            {
                new Rat(2, 1, "rat",1),
                new Rat(1, 1, "rat",1),
                new Rat(1, 1, "rat",1)
            };
            monsterRoster.AddMonsterToRoster(1, monstrRoster.ToArray());
            monsterRoster.AddMonsterToRoster(1, monstrRoster2.ToArray());
            //monsterRoster.AddMonsterToRoster(1, monsterRoster.generateRandomMonsters(1, 100).ToArray());
            quests.SignOnQuest(new KillQuest("rats problem", "kill 3 rats", monstrRoster, new RewardModel { XpReward = 10, GoldReward = 15 },
                new BaseQuestModel[]{
                    new KillQuest("rats problem 2", "kill 3 rats", monstrRoster2, new RewardModel { XpReward = 100, GoldReward = 115 })
                },quests.UpdateQuestListFromChildQuests));
            battleClass = new someclass(Pers, monsterRoster.GetMonsterRosterForFloor(1).ToList());

            this.timeNow.Content = $"Сейчас: {DateTime.Now.ToShortTimeString()}";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            LoadGrids();
            SetBindingsProperties();
        }

        private void SetBindingsProperties()
        {
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
                Binding b3 = new Binding();
                b3.Source = Pers;
                b3.Path = new PropertyPath(nameof(pp.InventoryList));
                b3.Mode = BindingMode.OneWay;
                this.inventoryView.SetBinding(DataGrid.ItemsSourceProperty, b3);
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
                gameProcess.MoveNext(monsterRoster.GetMonsterRosterForFloor(1));
            }
            else if (gameProcess.CurrentState == GameState.BackToTown)
            {
                //anotherMessage = "Возвращаемся в город" + Environment.NewLine;
                gameProcess.MoveNext(null);
            }
            else if (gameProcess.CurrentState == GameState.Healing)
            {
                //anotherMessage = "Лечимся" + Environment.NewLine;
                gameProcess.MoveNext(null);
            }
            else if (gameProcess.CurrentState == GameState.Looting)
            {
                gameProcess.MoveNext(lootManager.GetNextLootableObject());
            }
            else if(gameProcess.CurrentState == GameState.Explorirng)
            {
                if (quests.ActiveQuests() == 0)
                {
                    quests.SignOnQuest(quests.GenerateQuest());
                    battleClass.Enemies = monsterRoster.GetMonsterRosterForFloor(1).ToList();
                }
                gameProcess.MoveNext(null);
            }
            else
            {
                GameManager.GameTick(new IUpdatable[] { quests });
                gameProcess.MoveNext(null);
            }
            FightMessage = GameManager.GetUnreadLogsString(GameNotificationState.Battle);
            QuestMessage = GameManager.GetUnreadLogsString(GameNotificationState.Quest);
            anotherMessage = GameManager.GetUnreadLogsString(GameNotificationState.Other);
            allMessage = GameManager.GetUnreadLogsString(GameNotificationState.All);

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
            public List<AbstractPerson> Enemies;

            public someclass(AbstractPerson pers, List<AbstractPerson> enemies)
            {
                this.Pers = pers;
                this.Enemies = enemies;
            }
            public GameNotification Update()
            {
                GameNotification notification = new GameNotification()
                {
                    GameNotificationState = GameNotificationState.Battle
                };
                if (Enemies.Count == 0) return notification;
                var Enemy = Enemies.First();
                if (Enemy is IDmgable dmg)
                {
                    Pers.MakeDmg(dmg);
                    notification.Body += @$"Наносим {Pers.AttackAmount()} урона по противнику [{Enemy}]!" + Environment.NewLine;
                }
                if (Enemy.IsDead())
                {
                    notification.Body += @$"Противник [{Enemy}] получает смертельную рану!" + Environment.NewLine;
                    if (Enemy is IXPTransmittable xpTransmittable)
                    {
                        if (Pers is IXPRecievable xpRecievable)
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
                        notification.Body += $@"Противник [{Enemy}] наносит {Enemy.AttackAmount()} урона по герою!" + Environment.NewLine;
                    }
                }
                Enemies.RemoveAll(x => x.IsDead());
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
        private void LoadGridInventory()
        {
            //this.inventoryView.HeadersVisibility = DataGridHeadersVisibility.None;
        }

        private void LoadGrids()
        {
            LoadGridHero();
            LoadGridInventory();
        }

        private void expBar_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (Pers is AbstractPersonTier0 ap)
            {
                ((ProgressBar)e.Source).ToolTip = $"exp for next level: {ap.MaxXP - ap.CurrentXP} exp.";
            }

        }
    }
}
