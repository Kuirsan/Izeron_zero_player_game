using System;
using System.Collections.Generic;

namespace Izeron.Library.Persons
{
    public static class HeroNameGenerator
    {
        private static readonly string[] FirstNames = new[]
        {
            "Артур", "Бенедикт", "Вальтер", "Гарольд", "Дункан",
            "Эдвард", "Фелиус", "Гвидо", "Хьюго", "Ингвар",
            "Якоб", "Кайл", "Ларс", "Маркус", "Натан",
            "Оливер", "Патрик", "Квентин", "Рейнальд", "Себастьян",
            "Тобиас", "Ульрих", "Виктор", "Вильям", "Ксавьер"
        };

        private static readonly string[] Surnames = new[]
        {
            "Храбрый", "Железный", "Стойкий", "Бесстрашный", "Верный",
            "Могучий", "Быстрый", "Мудрый", "Сильный", "Отважный",
            "Грозный", "Справедливый", "Яростный", "Благородный", "Непобедимый",
            "Защитник", "Воитель", "Странник", "Охотник", "Рыцарь"
        };

        private static Random _random = new Random();

        public static string Generate()
        {
            string firstName = FirstNames[_random.Next(FirstNames.Length)];
            string surname = Surnames[_random.Next(Surnames.Length)];
            return $"{firstName} {surname}";
        }
    }
}
