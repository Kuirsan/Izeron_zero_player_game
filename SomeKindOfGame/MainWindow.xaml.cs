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
using GameLogic.Library.FloorSystem;
using Izeron.Library.Enums;
using Izeron.Library.Exceptions;
using Izeron.Library.Interfaces;
using Izeron.Library.Notification;
using Izeron.Library.Perks;
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
        BattleController battleClass;
        QuestObserver quests;
        GameProcess gameProcess;
        BattleRosterManager monsterRoster = new BattleRosterManager();
        LootManager lootManager = new LootManager();
        PerkManager perkManager = new PerkManager();
        FloorManager floorManager = new FloorManager();
        DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();

            InitializeGame();
        }

        private void InitializeGame()
        {
            monsterRoster.NotifyLootSystem += lootManager.GenerateLootAndAddByFloor;
            monsterRoster.OnMonstersKilled += (count) => 
            {
                for (int i = 0; i < count; i++)
                {
                    floorManager.RegisterMonsterKill();
                }
            };
            
            // Подписываемся на переход на новый этаж для генерации врагов
            floorManager.OnFloorAdvanced += (newFloor) =>
            {
                int heroPower = 0;
                if (Pers is AbstractPersonTier0 heroTier0)
                {
                    heroPower = HeroPowerCalculator.CalculatePowerRating(Pers);
                }
                
                // Генерируем 20-30 монстров на новом этаже
                var monstersToSpawn = new Random().Next(20, 31);
                var newMonsters = monsterRoster.GenerateRandomMonstersByPower(newFloor, monstersToSpawn, heroPower);
                monsterRoster.AddMonsterToRoster(newFloor, newMonsters.ToArray());
                
                // Уведомляем в логах
                var notification = new GameNotification
                {
                    GameNotificationState = GameNotificationState.Other,
                    Body = $"Вы спустились на {newFloor} этаж! Враги становятся сильнее..." + Environment.NewLine
                };
                GameManager.AddNotification(notification);
            };

            Dictionary<int, float> dict = new Dictionary<int, float>
            {
                {0,11f},{1,20f},{2,30f},{3,40f},{4,50f},{5,60f},{6,70f},{7,80f},{8,90f},{9,100f},{10,130f}
            };
            Pers = new Peasant(2, dict); // Начинаем с 2 силы для лучшего баланса


            quests = GameManager.InitiateQuestObserver(Pers,monsterRoster);
            gameProcess = GameManager.InitiateGameProcess(Pers, new GameStateLogicByHero());
            
            // Процедурная генерация начального квеста
            var initialQuest = quests.GenerateQuest();
            quests.SignOnQuest(initialQuest);
            
            // Инициализируем battleClass с врагами с текущего этажа (если они есть)
            var initialMonsters = monsterRoster.GetMonsterRoastForFloor(floorManager.CurrentFloor);
            battleClass = new BattleController(Pers, initialMonsters != null ? initialMonsters.ToList() : new List<AbstractPerson>());


            this.timeNow.Content = $"Сейчас: {DateTime.Now.ToShortTimeString()}";
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += timer_Tick;
            gameTimer.Start();
            LoadGrids();
            Pers.IsQuestItem = (itemName) => {
                var activeQuests = quests.GetActiveQuests();
                foreach (var quest in activeQuests)
                {
                    if (quest is CollectLootQuest collectQuest)
                    {
                        if (collectQuest.RequiredLootName == itemName && !collectQuest.IsFinish)
                        {
                            return true;
                        }
                    }
                }
                return false;
            };
            
            // Подписываемся на событие повышения уровня
            if (Pers is AbstractPersonTier0 heroTier0)
            {
                heroTier0.LeveledUp += OnHeroLeveledUp;
            }
            
            // Отображаем имя героя с регистрацией
            string baseName = HeroNameGenerator.Generate();
            string heroName = HeroRegistry.RegisterHero(baseName);
            Pers.Name = heroName;
            heroNameLabel.Content = heroName;
            
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
                
                Binding b4 = new Binding();
                b4.Source = Pers;
                b4.Path = new PropertyPath(nameof(pp.PerksList));
                b4.Mode = BindingMode.OneWay;
                this.perksView.SetBinding(DataGrid.ItemsSourceProperty, b4);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {

            string dateTime = DateTime.Now.ToShortTimeString();
            this.timeNow.Content = $"Сейчас: {dateTime}";
            this.gameStateLabel.Content = $"Состояние: {GetGameStateRussianName(gameProcess.CurrentState)}";
            this.floorInfoLabel.Content = floorManager.GetFloorInfo();
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
                    gameTimer.Stop();
                    var result = MessageBox.Show(ex.Message + "\n\nStart a new game?", "Game Over", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        ResetGame();
                    }
                    return;
                }
                var monstersForFloor = monsterRoster.GetMonsterRoastForFloor(floorManager.CurrentFloor);
                UpdateCurrentEnemy();
                gameProcess.MoveNext(monstersForFloor ?? new List<AbstractPerson>());
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
                // Если нет активных квестов, генерируем новый
                var activeQuests = quests.GetActiveQuests();
                bool hasKillQuest = activeQuests.Any(q => q is KillQuest);
                bool hasCollectQuest = activeQuests.Any(q => q is CollectLootQuest);

                if (!hasKillQuest)
                {
                    var newQuest = quests.GenerateQuestByType("Kill");
                    quests.SignOnQuest(newQuest);
                }

                if (!hasCollectQuest)
                {
                    var newQuest = quests.GenerateQuestByType("CollectLoot");
                    quests.SignOnQuest(newQuest);
                }

                var currentMonsters = monsterRoster.GetMonsterRoastForFloor(1);
                if (currentMonsters != null && currentMonsters.Count > 0)
                {
                    battleClass.Enemies = currentMonsters.ToList();
                }
                else
                {
                    battleClass.Enemies = new List<AbstractPerson>();
                }
                gameProcess.MoveNext(null);
            }
            else
            {
                gameProcess.MoveNext(null);
            }
            
            // Обновляем квесты на каждом тике
            GameManager.GameTick(new IUpdatable[] { quests });
            
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

            // Обновляем список активных квестов
            this.questsView.ItemsSource = quests.GetActiveQuests();
            this.questsView.Items.Refresh();

        }

        /// <summary>
        /// Контроллер боевой системы - управляет сражением между героем и врагами
        /// </summary>
        class BattleController : IUpdatable
        {
            AbstractPerson Pers;
            public List<AbstractPerson> Enemies;

            public BattleController(AbstractPerson pers, List<AbstractPerson> enemies)
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

        private void OnHeroLeveledUp(object sender, int newLevel)
        {
            if (Pers is AbstractPersonTier0 hero)
            {
                var perk = perkManager.GetRandomPerkForLevel(newLevel);
                if (perk != null)
                {
                    hero.AddPerk(perk);
                    
                    // Логируем получение перка
                    var notification = new GameNotification
                    {
                        GameNotificationState = GameNotificationState.Other,
                        Body = $"Получен новый перк: {perk.Name} (+{perk.Value} {perk.Type})!" + Environment.NewLine
                    };
                    GameManager.AddNotification(notification);
                }
            }
        }

        private string GetGameStateRussianName(GameState state)
        {
            return state switch
            {
                GameState.InTown => "В городе",
                GameState.Fighting => "Бой",
                GameState.Explorirng => "Исследование",
                GameState.Looting => "Сбор добычи",
                GameState.BackToTown => "Возвращение в город",
                GameState.Healing => "Лечение",
                GameState.SellingLoot => "Продажа добычи",
                GameState.BuyGears => "Покупка снаряжения",
                _ => state.ToString()
            };
        }

        private void ResetGame()
        {
            // Очищаем старые данные
            monsterRoster = new BattleRosterManager();
            lootManager = new LootManager();
            perkManager = new PerkManager();
            floorManager.Reset();
            
            // Реинициализируем игру
            InitializeGame();
            
            // Перезапускаем таймер
            gameTimer.Start();
            
            // Обновляем UI
            LoadGrids();
        }

        private void UpdateCurrentEnemy()
        {
            if (battleClass != null && battleClass.Enemies != null && battleClass.Enemies.Count > 0)
            {
                var currentEnemy = battleClass.Enemies.FirstOrDefault(e => !e.IsDead());
                if (currentEnemy != null)
                {
                    int currentHealth = currentEnemy.CurrentHealth;
                    int maxHealth = currentEnemy.MaxHealth;
                    int enemyDamage = currentEnemy.AttackAmount();
                    currentEnemyLabel.Content = $"{currentEnemy.Name} (HP: {currentHealth}/{maxHealth}, DMG: {enemyDamage})";
                }
                else
                {
                    currentEnemyLabel.Content = "No enemy";
                }
            }
            else
            {
                currentEnemyLabel.Content = "-";
            }
        }
    }
}
