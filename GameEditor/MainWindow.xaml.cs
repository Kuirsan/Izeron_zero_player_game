using GameLogic.Library;
using GameLogic.Library.LogicModels;
using Izeron.Library.Enums;
using Izeron.Library.Persons;
using Izeron.Library.Persons.Enemies.Tier0;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
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

namespace GameEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string fileName = @"C:\Users\ksmer\source\repos\SomeKindOfGame\SomeKindOfGame\bin\Debug\netcoreapp3.1\EnemiesLibrary\enemies.json";
            //serializeAndWrite(fileName);
            var enemies = getEnemyModelsFromJSON(fileName);
            foreach (var enemy in enemies)
            {

            }
        }
        private void serializeAndWrite(string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            GameEnemiesModel enemyModel = new GameEnemiesModel
               (PossibleTags: new SpecialEnemyTag[] {new SpecialEnemyTag(SpecialEnemyTags.Big)
                    ,new SpecialEnemyTag(SpecialEnemyTags.Small)
                    ,new SpecialEnemyTag(SpecialEnemyTags.Strong)
                    ,new SpecialEnemyTag(SpecialEnemyTags.Weak)},
                HPRange: new ParameterRange(1, 5),
                FloorRange: new ParameterRange(1, 2),
                enemyTypetags: null);
            GameEnemiesModel enemyModel2 = new GameEnemiesModel
              (PossibleTags: new SpecialEnemyTag[] {new SpecialEnemyTag(SpecialEnemyTags.Big)
                    ,new SpecialEnemyTag(SpecialEnemyTags.Small)
                    ,new SpecialEnemyTag(SpecialEnemyTags.Strong)
                    ,new SpecialEnemyTag(SpecialEnemyTags.Weak)},
               HPRange: new ParameterRange(1, 5),
               FloorRange: new ParameterRange(1, 2),
               enemyTypetags: null);
            List<GameEnemiesModel> enemyList = new List<GameEnemiesModel>();
            enemyList.Add(enemyModel); enemyList.Add(enemyModel2);
            string jsonString = JsonSerializer.Serialize(enemyList, options);
            File.AppendAllText(path, jsonString);
        }
        private List<GameEnemiesModel> getEnemyModelsFromJSON(string path)
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<GameEnemiesModel>>(jsonString);
        }
    }
    
}
