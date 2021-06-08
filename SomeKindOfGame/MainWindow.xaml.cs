using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Persons;
using Izeron.Library.Persons.Enemies.Tier0;
using Izeron.Library.Persons.Tier0;
using QuesHandlerSystem.Library;
using QuestHandlerSystem.Library.Quest.Models;
using SomeKindOfGame.GameCenter;

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
                {0,10f},{1,20f},{2,30f},{3,40f},{4,50f},{5,60f},{6,70f},{7,80f},{8,90f},{9,100f},{10,130f}
            };
            Pers = new Peasant(1, dict);
            Enemy = new Peasant(1, dict);
            quests = GameManager.InitiateQuestObserver(Pers);
            List<AbstractPerson> monsterRoaster = new List<AbstractPerson>(){
                new Rat(1, 1, "rat"),
                new Rat(1, 1, "rat"),
                new Rat(1, 1, "rat")
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
                //this.doSomething.Content = $"XP is {pp.CurrentXP}!";
                //this.doSomething.SetBinding(Label.ContentProperty, b);
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    txtBlock.Text += $"Создан персонаж класса {Pers} а также его враг класса {Enemy}\n";
        //    if (Enemy is IDmgable dmgable)
        //    {
        //        txtBlock.Text += $"Наносим урон \n";
        //        Pers.MakeDmg(dmgable);
        //    }
        //    else
        //    {
        //        txtBlock.Text += $"Нельзя ударить \n";
        //    }
        //    if (Pers is IXPRecievable pers) pers.ReceiveXP(7);
        //    if (Enemy is IXPRecievable enemy) enemy.ReceiveXP(17);
        //}
        void timer_Tick(object sender, EventArgs e)
        {
            this.timeNow.Content = $"Сейчас: {DateTime.Now.ToShortTimeString()}";
            try
            {
                GameManager.GameTick(new IUpdatable[] { battleClass,quests});
            }
            catch(YouDeadException ex)
            {
                MessageBox.Show(ex.Message);
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
            public void Update()
            {
                //if (Pers is IXPRecievable xP)
                //{
                //    xP.ReceiveXP(3);
                //    //this.expBar.Value = xP.GetCurrentXP();
                //}
                if (Enemies.Count == 0) return;
                var Enemy = Enemies.First();
                if (Enemy is IDmgable dmg)
                {
                    dmg.GetDamage(1);
                }
            }
        }


        private void DoSomething(float sec, int msDuratation)
        {
            double timeRemain = sec;
            
            try
            {
                while (timeRemain > 0)
                {
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(msDuratation);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.doSomething.Content = $"Do Something in {timeRemain.ToString("F2")} secs\n";
                    }), DispatcherPriority.Normal, cancelToken.Token);
                    Thread.Sleep(timeSpan);
                    timeRemain -= timeSpan.TotalSeconds;
                }
            }
            catch (OperationCanceledException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancelToken.Cancel();
        }
        private void LoadGridHero()
        {
            //Binding b = new Binding();
            //b.Source = Pers;
            //b.Path = new PropertyPath("exp");
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
