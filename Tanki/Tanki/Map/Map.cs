using System.Collections.Generic;
using Tanki.Tanks.Enemy;

namespace Tanki.Map
{
    public class GameMap
    {
        private char[,] cells;
        private List<EnemyTank> enemyTanks; // Список врагов на карте

        public int Width { get; private set; } // Свойство для ширины карты
        public int Height { get; private set; } // Свойство для высоты карты

        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new char[height, width];
            enemyTanks = new List<EnemyTank>(); // Инициализируем список врагов
            InitializeMap();
        }

        private void InitializeMap()
        {
            string[] mapData = new string[]
            {
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓                            ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓                            ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓████    ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓████    ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓            ▓▓▓▓            ▓▓▓▓            ▓▓▓▓",
                "▓▓▓▓            ▓▓▓▓            ▓▓▓▓            ▓▓▓▓",
                "▓▓▓▓▓▓▓▓        ████    ████████████▓▓▓▓▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓▓▓▓▓        ████    ████████████▓▓▓▓▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓            ▓▓▓▓            ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓            ▓▓▓▓            ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓████    ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓████    ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓                                              ▓▓▓",
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓"
            };

            for (int y = 0; y < cells.GetLength(0); y++)
            {
                for (int x = 0; x < cells.GetLength(1); x++)
                {
                    cells[y, x] = mapData[y][x]; // Инициализируем карту из данных
                }
            }
        }

        public void Render(bool clear = false)
        {
            if (clear)
            {
                Console.Clear();
            }

            for (int y = 0; y < cells.GetLength(0); y++)
            {
                for (int x = 0; x < cells.GetLength(1); x++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(cells[y, x]);
                }
            }
        }

        public bool HasEnemyAt(int x, int y)
        {
            // Проверяем, есть ли враг в заданной позиции
            foreach (var enemy in enemyTanks)
            {
                if (enemy.X == x && enemy.Y == y && enemy.IsAlive) // Предполагается, что у врага есть свойство IsAlive
                {
                    return true; // Враг найден
                }
            }
            return false; // Врагов в данной позиции нет
        }

        public void AddEnemy(EnemyTank enemy)
        {
            enemyTanks.Add(enemy); // Метод для добавления врага на карту
        }

        public bool CanMoveTo(int x, int y)
        {
            // Проверяем границы карты
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false; // Выход за границы
            }

            // Проверяем, является ли ячейка стеной или разрушимым объектом
            return cells[y, x] == ' ' || cells[y, x] == '▓'; // Проходим только через пробелы и непробиваемые стены
        }

        public bool CanShootThrough(int x, int y)
        {
            // Проверяем, можно ли прострелить блок
            // Проверка границ перед доступом к ячейке
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false; // Выход за границы
            }
            return cells[y, x] == '▓'; // Можно стрелять через блоки, обозначенные символом '▓'
        }

        public void DestroyBlock(int x, int y)
        {
            // Метод для разрушения блока, если он разрушим
            if (cells[y, x] == '█') // Если блок разрушим
            {
                cells[y, x] = ' '; // Заменяем блок на пустое пространство
            }
        }
    }
}