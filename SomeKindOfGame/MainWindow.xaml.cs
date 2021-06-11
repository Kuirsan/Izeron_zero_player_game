using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using GameCenter.Library.GameCenter;
using GameLogic.Library.GameStateLogic;
using Izeron.Library.Enums;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
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
        List<AbstractPerson> monsterRoaster;

        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
        public MainWindow()
        {

            Dictionary<int, float> dict = new Dictionary<int, float>
            {
                {0,11f},{1,20f},{2,30f},{3,40f},{4,50f},{5,60f},{6,70f},{7,80f},{8,90f},{9,100f},{10,130f}
            };
            Pers = new Peasant(1, dict);
            Enemy = new Peasant(1, dict);
            quests = GameManager.InitiateQuestObserver(Pers);
            gameProcess = GameManager.InitiateGameProcess(Pers, new GameStateLogicByHero());
            monsterRoaster = new List<AbstractPerson>(){
                new Rat(1, 1, "rat",1),
                new Rat(2, 1, "rat",1),
                new Rat(3, 1, "rat",1),
                new Rat(5, 1, "giant rat",1)
                };
            quests.SignOnQuest(new KillQuest("rats problem", "kill 3 rats", monsterRoaster, new RewardModel { xpReward = 10 }));
            battleClass = new someclass(Pers, monsterRoaster);
            InitializeComponent();
            this.timeNow.Content = $"Сейчас: {DateTime.Now.ToShortTimeString()}";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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
            this.timeNow.Content = $"Сейчас: {DateTime.Now.ToShortTimeString()}";
            string message = string.Empty;
            if (gameProcess.CurrentState == GameState.Fighting)
            {
                try
                {
                    message=GameManager.GameTick(new IUpdatable[] { battleClass, quests });
                }
                catch (YouDeadException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                gameProcess.MoveNext(monsterRoaster);
            }
            else
            {
                message=(GameManager.GameTick(new IUpdatable[] { quests }));
                gameProcess.MoveNext(null);
            }
            if (!string.IsNullOrEmpty(message.Trim()))
            {
                this.notificationText.Text += message;
                this.textViewerScroll.ScrollToEnd();
            }
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
            public string Update()
            {
                string notification = string.Empty;
                if (Enemies.Count == 0) return notification;
                var Enemy = Enemies.First();
                if (Enemy is IDmgable dmg)
                {
                    Pers.MakeDmg(dmg);
                    notification += @$"Наносим {Pers.attackAmount()} урона по противнику [{Enemy}]!" + Environment.NewLine;
                }
                if(Enemy.isDead())
                {
                    notification += @$"Противник [{Enemy}] получает смертельную рану!" + Environment.NewLine;
                }
                else
                {
                    if (Pers is IDmgable pdmg)
                    {
                        Enemy.MakeDmg(pdmg);
                        notification += $@"Противник [{Enemy}] наносит {Enemy.attackAmount()} урона по герою!" + Environment.NewLine;
                    }
                }
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
