using System;
using Tanki;

namespace Tanki
{
    public class Renderer
    {
        public int width { get; private set; }
        public int height { get; private set; }

        private const int MaxColors = 8;
        private readonly ConsoleColor[] _colors;
        private readonly char[,] _pixels;
        private readonly byte[,] _pixelColors;
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public ConsoleColor bgColor { get; set; }

        public char this[int w, int h]
        {
            get { return _pixels[w, h]; }
            set { _pixels[w, h] = value; }
        }

        public Renderer(ConsoleColor[] colors)
        {
            if (colors.Length > MaxColors)
            {
                var tmp = new ConsoleColor[MaxColors];
                Array.Copy(colors, tmp, tmp.Length);
                colors = tmp;
            }

            _colors = colors;

            _maxWidth = Console.LargestWindowWidth;
            _maxHeight = Console.LargestWindowHeight;
            width = Console.WindowWidth;
            height = Console.WindowHeight;

            _pixels = new char[_maxWidth, _maxHeight];
            _pixelColors = new byte[_maxWidth, _maxHeight];
        }

        public void SetPixel(int x, int y, char symbol, int colorIndex)
        {
            if (x < 0 || x >= _maxWidth || y < 0 || y >= _maxHeight || colorIndex < 0 || colorIndex >= MaxColors)
                return; // Проверка выхода за пределы массива

            _pixels[x, y] = symbol;
            _pixelColors[x, y] = (byte)colorIndex;
        }

        public void Render()
        {
            Console.Clear();
            Console.BackgroundColor = bgColor;

            for (var w = 0; w < width; w++)
                for (var h = 0; h < height; h++)
                {
                    var colorIdx = _pixelColors[w, h];
                    var color = _colors[colorIdx];
                    var symbol = _pixels[w, h];

                    if (symbol == 0 || color == bgColor)
                        continue;

                    Console.ForegroundColor = color;

                    Console.SetCursorPosition(w, h);
                    Console.Write(symbol);
                }

            Console.ResetColor();
            Console.CursorVisible = false;
        }

        public void DrawMap(GameState gameState)
        {
            if (gameState?.GameMap == null) return; // Проверка на null

            for (int y = 0; y < 50; y++) // Изменение высоты карты на 30
            {
                for (int x = 0; x < 60; x++) // Изменение ширины карты на 40
                {
                    char mapSymbol = gameState.GameMap.GetMapSymbol(x, y);
                    SetPixel(x, y, mapSymbol, 0); // Используйте индекс цвета 0 для карты
                }
            }
        }

        public void DrawBullets(GameState gameState)
        {
            foreach (Bullet bullet in gameState.Bullets)
            {
                if (bullet != null && bullet.X >= 0 && bullet.X < width && bullet.Y >= 0 && bullet.Y < height)
                {
                    SetPixel(bullet.X, bullet.Y, bullet.Symbol, 1); // Используйте индекс цвета 1 для снарядов
                }
            }
        }

        public void DrawEnemies(GameState gameState)
        {
            foreach (var enemyTank in gameState.EnemyTanks)
            {
                if (enemyTank != null)
                {
                    enemyTank.Draw(this); // Рисуем врага, передавая текущий рендерер
                }
            }
        }

        public void DrawTank(GameState gameState)
        {
            if (gameState?.PlayerTank != null)
            {
                gameState.PlayerTank.Draw(this); // Рисуем танк, передавая текущий рендерер
            }
        }

        public void Draw(GameState gameState)
        {
            DrawMap(gameState); // Рисуем карту
            DrawBullets(gameState); // Рисуем снаряды
            DrawTank(gameState); // Рисуем танк
            DrawEnemies(gameState); // Рисуем врагов
            Render(); // Отображаем всё на экране
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Renderer casted)
                return false;

            if (_maxWidth != casted._maxWidth || _maxHeight != casted._maxHeight ||
                width != casted.width || height != casted.height ||
                _colors.Length != casted._colors.Length)
            {
                return false;
            }

            for (int i = 0; i < _colors.Length; i++)
            {
                if (_colors[i] != casted._colors[i])
                    return false;
            }

            for (int w = 0; w < width; w++)
                for (var h = 0; h < height; h++)
                {
                    if (_pixels[w, h] != casted._pixels[w, h] ||
                                    _pixelColors[w, h] != casted._pixelColors[w, h])
                    {
                        return false;
                    }
                }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Combine(_maxWidth, _maxHeight, width, height);

            for (int i = 0; i < _colors.Length; i++)
            {
                hash = HashCode.Combine(hash, _colors[i]);
            }

            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                {
                    hash = HashCode.Combine(hash, _pixelColors[w, h], _pixels[w, h]);
                }

            return hash;
        }
    }
}