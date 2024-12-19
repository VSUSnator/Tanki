using System;
using System.Collections.Generic;
using Tanki.Tanks;

namespace Tanki
{
    public class Renderer
    {
        private int mapWidth;
        private int mapHeight;

        public Renderer(int mapWidth, int mapHeight)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
        }

        public void Render(Tank playerTank, List<Bullet> bullets)
        {
            Console.Clear();
            // Отображение карты (можно добавить логику для отображения карты)

            // Получение символов для отображения танка в зависимости от его направления
            string[] tankSymbols = GetTankSymbols(playerTank.Direction);

            // Отображение танка
            for (int i = 0; i < tankSymbols.Length; i++)
            {
                Console.SetCursorPosition(playerTank.X, playerTank.Y + i);
                Console.Write(tankSymbols[i]);
            }

            // Отображение снарядов
            foreach (var bullet in bullets)
            {
                Console.SetCursorPosition(bullet.BulletX, bullet.BulletY);
                Console.Write("B"); // Символ для снаряда
            }
        }

        private string[] GetTankSymbols(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new string[]
                    {
                    "  ^  ",
                    "  |  ",
                    " /O\\ ",
                    "=====",
                    };
                case Direction.Down:
                    return new string[]
                    {
                    "=====",
                    " \\|/ ",
                    "  v  ",
                    };
                case Direction.Left:
                    return new string[]
                    {
                    " /\\  ",
                    " |===|",
                    " /    ",
                    };
                case Direction.Right:
                    return new string[]
                    {
                    "  /\\ ",
                    "|===| ",
                    "    \\ ",
                    };
                default:
                    return new string[] { "  ^  ", " /|\\ ", "=====" }; // По умолчанию
            }
        }
    }
}