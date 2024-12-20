using System;
using System.Collections.Generic;
using Tanki.Tanks;
using Tanki.Tanks.Enemy;

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

            // Отображение танка игрока
            string[] tankSymbols = GetTankSymbols(playerTank.Direction);
            for (int i = 0; i < tankSymbols.Length; i++)
            {
                Console.SetCursorPosition(playerTank.X, playerTank.Y + i);
                Console.Write(tankSymbols[i]); // Выводим символы танка игрока
            }

            // Отображение снарядов
            foreach (var bullet in bullets)
            {
                // Проверка, что снаряд находится в пределах карты
                if (bullet.BulletX >= 0 && bullet.BulletX < mapWidth && bullet.BulletY >= 0 && bullet.BulletY < mapHeight)
                {
                    Console.SetCursorPosition(bullet.BulletX, bullet.BulletY);
                    Console.Write("*"); // Символ для снаряда
                }
            }
        }

        public void RenderEnemyTank(EnemyTank enemyTank)
        {
            string[] enemyTankSymbols = GetEnemyTankSymbols(enemyTank.Direction);
            for (int i = 0; i < enemyTankSymbols.Length; i++)
            {
                Console.SetCursorPosition(enemyTank.X, enemyTank.Y + i);
                Console.Write(enemyTankSymbols[i]); // Выводим символы врага
            }
        }

        private string[] GetTankSymbols(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new string[]
                    {
                        @"|^|", // Символ для игрока, смотрящего вверх
                    };
                case Direction.Down:
                    return new string[]
                    {
                        @"|v|", // Символ для игрока, смотрящего вниз
                    };
                case Direction.Left:
                    return new string[]
                    {
                        @"<|", // Символ для игрока, смотрящего влево
                    };
                case Direction.Right:
                    return new string[]
                    {
                        @"|>", // Символ для игрока, смотрящего вправо
                    };
                default:
                    return new string[] { "  ^  ", " /|\\ ", "=====" }; // По умолчанию
            }
        }

        private string[] GetEnemyTankSymbols(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new string[]
                    {
                        @"/^\\", // Символ для врага, смотрящего вверх
                    };
                case Direction.Down:
                    return new string[]
                    {
                        @"/v\\", // Символ для врага, смотрящего вниз
                    };
                case Direction.Left:
                    return new string[]
                    {
                        @"<\\", // Символ для врага, смотрящего влево
                    };
                case Direction.Right:
                    return new string[]
                    {
                        @"\\>", // Символ для врага, смотрящего вправо
                    };
                default:
                    return new string[] { "  ^  ", " /|\\ ", "=====" }; // По умолчанию
            }
        }
    }
}