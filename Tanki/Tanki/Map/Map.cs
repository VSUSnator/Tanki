using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

namespace Tanki.Map
{
    public class GameMap
    {
        private List<MapObject> mapObjects;
        private char[,] mapState;

        public GameMap(string filePath)
        {
            mapObjects = new List<MapObject>();
            LoadMap(filePath);
            InitializeMapState();
        }
        private void InitializeMapState()
        {
            mapState = new char[20, 15]; // Предполагаем, что размер карты 20x15
            foreach (var mapObject in mapObjects)
            {
                mapState[mapObject.X, mapObject.Y] = mapObject.Symbol;
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
            if (x >= 0 && x < 20 && y >= 0 && y < 15)
            {
                return mapState[x, y]; // Возвращаем символ из состояния карты
            }
            return '.'; // По умолчанию
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