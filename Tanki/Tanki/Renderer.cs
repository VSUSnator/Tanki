using System;
using Tanki.Map;

namespace Tanki
{
    public class Renderer
    {
        private GameState gameState;

        public Renderer(GameState gameState)
        {
            this.gameState = gameState;
        }

        public void Draw()
        {
            Console.Clear(); // Очистка консоли

            // Отображение карты
            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    Console.Write(gameState.GameMap.GetMapSymbol(x, y)); // Используем экземпляр GameMap из gameState
                }
                Console.WriteLine();
            }

            // Отображение снарядов
            foreach (Bullet bullet in gameState.Bullets) // Используем Bullets из gameState
            {
                Console.SetCursorPosition(bullet.X, bullet.Y);
                Console.Write(bullet.Symbol); // Отображение снаряда
            }

            // Отображение танка
            Console.SetCursorPosition(gameState.Tank.X, gameState.Tank.Y);
            Console.Write("T"); // Отображение танка
        }
    }
}