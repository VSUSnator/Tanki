using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki.Tanks;
using Tanki.Map;
using Tanki;

namespace Tanki.Map
{
    public class GameMap
    {
        private char[,] cells;


        public GameMap(int width, int height)
        {
            cells = new char[height, width];
            InitializeMap();
        }

        private void InitializeMap()
        {
            // Определяем размеры карты
            int width = 50; // ширина карты
            int height = 24; // высота карты
            cells = new char[height, width]; // инициализация двумерного массива

            // Задаем значения для каждой ячейки массива
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
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████    ████████████▓▓▓▓▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████    ████████████▓▓▓▓▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓            ▓▓▓▓            ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓            ▓▓▓▓            ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓████    ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓████    ▓▓▓▓    ▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓                    ▓▓▓▓            ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓                    ▓▓▓▓            ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓                            ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓    ▓▓▓▓                            ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓",
                "▓▓▓▓                    ▓▓▓▓                    ▓▓▓▓",
                "▓▓▓▓                    ▓▓▓▓                    ▓▓▓▓",
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
                "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓"
            };

            for (int y = 0; y < cells.GetLength(0); y++)
            {
                for (int x = 0; x < cells.GetLength(1); x++)
                {
                    cells[y, x] = ' '; // Заполняем пробелами
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
                    if (x >= 0 && x < cells.GetLength(1) && y >= 0 && y < cells.GetLength(0))
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(cells[y, x]);
                    }
                }
            }
        }
    }
}
