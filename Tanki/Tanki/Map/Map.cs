using System.Collections.Generic;
using System.IO;

namespace Tanki.Map
{
    public class GameMap
    {
        private List<MapObject> mapObjects;
        private char[,] mapState; // Представление состояния карты

        public int Width => mapState.GetLength(0); // Ширина карты
        public int Height => mapState.GetLength(1); // Высота карты

        public GameMap(string filePath)
        {
            mapObjects = new List<MapObject>();
            LoadMap(filePath);
            InitializeMapState();
        }

        private void InitializeMapState()
        {
            mapState = new char[20, 15]; // Предполагаем, что размер карты 20x15
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    mapState[x, y] = ' '; // Изначально заполняем пустыми символами
                }
            }

            // Нужно убедиться, что у нас есть все нужные объекты
            foreach (var mapObject in mapObjects)
            {
                if (mapObject.X >= 0 && mapObject.X < Width && mapObject.Y >= 0 && mapObject.Y < Height)
                {
                    mapState[mapObject.X, mapObject.Y] = mapObject.Symbol;
                }
            }
        }

        private void LoadMap(string filePath)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                int height = lines.Length;

                for (int y = 0; y < height; y++)
                {
                    int width = lines[y].Length;

                    for (int x = 0; x < width; x++)
                    {
                        char symbol = lines[y][x];
                        MapObject mapObject;

                        switch (symbol)
                        {
                            case '#':
                                mapObject = new Wall(x, y);
                                break;
                            case 'X':
                                mapObject = new Obstacle(x, y);
                                break;
                            case '.':
                            default:
                                mapObject = new EmptySpace(x, y);
                                break;
                        }

                        mapObjects.Add(mapObject);
                    }
                }
            }
        }

        public char GetMapSymbol(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return mapState[x, y]; // Теперь обращаемся к mapState
            }
            return ' '; // Пустой символ, если вне границ
        }

        public void DestroyObject(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                mapState[x, y] = '.'; // Заменяем символ 'X' на пустое пространство
            }
        }

        public bool IsWall(int x, int y)
        {
            char symbol = GetMapSymbol(x, y);
            return symbol == '#' || symbol == 'X'; // Предположим, что '#' и 'X' - это стены
        }

        public void UpdateMapObject(MapObject mapObject)
        {
            // Проверяем, находится ли объект в пределах карты
            if (mapObject.X >= 0 && mapObject.X < mapState.GetLength(0) && mapObject.Y >= 0 && mapObject.Y < mapState.GetLength(1))
            {
                mapState[mapObject.X, mapObject.Y] = mapObject.Symbol; // Обновляем состояние карты
            }
        }
    }
}