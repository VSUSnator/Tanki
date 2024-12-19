using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki.Tanks;
using Tanki.Map;
using Tanki;

namespace Tanki
{
    public class Game
    {
        private Tank mainPlayerTank; // Основной танк игрока
        private TankMovement tankController; // Контроллер движения танка
        private Renderer renderer; // Объект рендерера
        private List<Bullet> bullets = new List<Bullet>(); // Список снарядов
        private int mapWidth = 50; // Ширина карты
        private int mapHeight = 24; // Высота карты
        private GameMap gameMap; // Объект карты
        private int frameCount = 0;

        public Game()
        {
            mainPlayerTank = new Tank { X = mapWidth / 2, Y = mapHeight / 2 };
            tankController = new TankMovement(mainPlayerTank, mapWidth, mapHeight);
            renderer = new Renderer(mapWidth, mapHeight);
            gameMap = new GameMap();
            gameMap.Render();
        }

        public void Start()
        {
            //while (true)
            //{
            //    UpdateGameState();
            //    RenderGame();
            //    System.Threading.Thread.Sleep(100); // Пауза между кадрами
            //}

            while (true)
            {
                UpdateGameState();

                // Обновляем каждые 5 кадров
                if (frameCount % 5 == 0)
                {
                    RenderGame();
                }

                frameCount++;
                System.Threading.Thread.Sleep(100); // Пауза между кадрами
            }

        }

        private void UpdateGameState()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                // Обновляем направление танка
                UpdateTankDirection(mainPlayerTank, key);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        tankController.Move(Direction.North); // Двигаем танк вверх
                        break;
                    case ConsoleKey.DownArrow:
                        tankController.Move(Direction.South); // Двигаем танк вниз
                        break;
                    case ConsoleKey.LeftArrow:
                        tankController.Move(Direction.West); // Двигаем танк влево
                        break;
                    case ConsoleKey.RightArrow:
                        tankController.Move(Direction.East); // Двигаем танк вправо
                        break;
                    case ConsoleKey.Spacebar:
                        mainPlayerTank.Shoot(bullets); // Стрельба
                        break;
                }

                // Обновляем снаряды
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    var bullet = bullets[i];
                    bullet.UpdateBullet();
                    if (bullet.BulletX < 0 || bullet.BulletX >= mapWidth || bullet.BulletY < 0 || bullet.BulletY >= mapHeight)
                    {
                        bullets.RemoveAt(i); // Удаляем, если снаряд вышел за пределы карты
                    }
                }
            }
        }

        public void UpdateTankDirection(Tank playerTank, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    playerTank.Direction = Direction.North;
                    break;
                case ConsoleKey.DownArrow:
                    playerTank.Direction = Direction.South;
                    break;
                case ConsoleKey.LeftArrow:
                    playerTank.Direction = Direction.West;
                    break;
                case ConsoleKey.RightArrow:
                    playerTank.Direction = Direction.East;
                    break;
            }
        }

        private void RenderGame()
        {
            Console.SetCursorPosition(0, 0);
            gameMap.Render();
            renderer.Render(mainPlayerTank, bullets); // Исправлено на mainPlayerTank
        }
    }
}