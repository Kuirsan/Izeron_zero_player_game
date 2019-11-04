using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using SomeKindOfGame.Interfaces;
using SomeKindOfGame.Persons;
using SomeKindOfGame.Persons.Tier0;

namespace SomeKindOfGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AbstractPerson Pers;
        AbstractPerson Enemy;
        public MainWindow()
        {
            Dictionary<int, float> dict = new Dictionary<int, float>
            {
                {0,10f},{1,20f},{2,30f}
            };
            Pers = new Peasant(1, dict);
            Enemy = new Peasant(1, dict);
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtBlock.Text += $"Создан персонаж класса {Pers} а также его враг класса {Enemy}";
            if (Enemy is IDmgable dmgable)
            {
                txtBlock.Text += $"Наносим урон";
                Pers.MakeDmg(dmgable);
            }
            else
            {
                txtBlock.Text += $"Нельзя ударить";
            }
            if (Pers is IXPRecievable pers) pers.ReceiveXP(7);
            if (Enemy is IXPRecievable enemy) enemy.ReceiveXP(17);
        }
    }
}
