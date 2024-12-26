using System.Collections.Generic;
using System.IO;

namespace Tanki.Map
{
    public class GameMap
    {
        private List<MapObject> mapObjects;
        private char[,] mapState; // Представление состояния карты
        private MapSize size; // Поле для хранения размера карты

        public MapSize Size => size; // Свойство для доступа к размеру карты
        public int Width => size.Width; // Ширина карты
        public int Height => size.Height; // Высота карты

        public GameMap(string filePath)
        {
            mapObjects = new List<MapObject>();
            LoadMap(filePath);
            InitializeMapState();
        }

        private void InitializeMapState()
        {
            mapState = new char[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    mapState[x, y] = ' '; // Изначально заполняем пустыми символами
                }
            }

            foreach (var mapObject in mapObjects)
            {
                if (IsInBounds(mapObject.X, mapObject.Y))
                {
                    mapState[mapObject.X, mapObject.Y] = mapObject.Symbol;
                }
            }
        }

        private void LoadMap(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var lines = File.ReadAllLines(filePath);
                    int height = lines.Length;

                    int width = 0;
                    foreach (var line in lines)
                    {
                        if (line.Length > width)
                        {
                            width = line.Length;
                        }
                    }

                    size = new MapSize(width, height);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            char symbol = (x < lines[y].Length) ? lines[y][x] : ' ';
                            MapObject mapObject;

                            switch (symbol)
                            {
                                case '#':
                                    mapObject = new Wall(x, y);
                                    break;
                                case '█':
                                    mapObject = new Water(x, y);
                                    break;
                                case '▓':
                                    mapObject = new Obstacle(x, y);
                                    break;
                                case ' ':
                                default:
                                    mapObject = new EmptySpace(x, y);
                                    break;
                            }

                            if (mapObject != null)
                            {
                                mapObjects.Add(mapObject);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Логирование или обработка ошибки
                    Console.WriteLine($"Ошибка при загрузке карты: {ex.Message}");
                }
            }
        }

        public char GetMapSymbol(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                return mapState[x, y];
            }
            return ' ';
        }

        public void SetMapSymbol(int x, int y, char symbol)
        {
            if (IsInBounds(x, y))
            {
                mapState[x, y] = symbol; // Установка символа на карте
            }
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void DestroyObject(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                mapState[x, y] = '.'; // Заменяем символ на пустое пространство
                // Здесь можно удалить или обновить объект в mapObjects
            }
        }

        public bool IsWall(int x, int y)
        {
            return IsInBounds(x, y) && (GetMapSymbol(x, y) == '#' || GetMapSymbol(x, y) == 'X');
        }

        public void UpdateMapObject(MapObject mapObject)
        {
            if (IsInBounds(mapObject.X, mapObject.Y))
            {
                mapState[mapObject.X, mapObject.Y] = mapObject.Symbol;
            }
        }
    }
}