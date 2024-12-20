using System;
using System.Collections.Generic;
using Tanki.Tanks;
using Tanki.Map;
using Tanki.Tanks.Enemy;
using Tanki.Tanks.Player;

namespace Tanki
{
    public class Game
    {
        private PlayerTank mainPlayerTank;
        private List<EnemyTank> enemyTanks;
        private TankMovement tankController;
        private Renderer renderer;
        private List<Bullet> bullets = new List<Bullet>();
        public static int MapWidth { get; private set; } = 50;
        public static int MapHeight { get; private set; } = 24;
        private GameMap gameMap;
        private int frameCount = 0;

        public Game()
        {
            mainPlayerTank = new PlayerTank(MapWidth / 2, MapHeight / 2, Direction.Up);
            enemyTanks = new List<EnemyTank>
            {
                new EnemyTank(MapWidth / 4, MapHeight / 4, Direction.Right),
                new EnemyTank(MapWidth / 3, MapHeight / 3, Direction.Left)
            };
            tankController = new TankMovement(mainPlayerTank, MapWidth, MapHeight);
            renderer = new Renderer(MapWidth, MapHeight);
            gameMap = new GameMap(MapWidth, MapHeight);
        }

        public void Start()
        {
            while (true)
            {
                UpdateGameState();

                // Обновляем каждые 5 кадров
                if (frameCount % 4 == 0)
                {
                    RenderGame();
                }

                frameCount++;
                System.Threading.Thread.Sleep(100); // Пауза между кадрами

                // Добавляем возможность выхода из игры
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    break; // Выход из игры по нажатию Escape
                }
            }
        }

        private void UpdateGameState()
        {
            if (tankController == null)
            {
                throw new InvalidOperationException("tankController не инициализирован.");
            }

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                UpdateTankDirection(mainPlayerTank, key);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        tankController.Move(Direction.North);
                        break;
                    case ConsoleKey.DownArrow:
                        tankController.Move(Direction.South);
                        break;
                    case ConsoleKey.LeftArrow:
                        tankController.Move(Direction.West);
                        break;
                    case ConsoleKey.RightArrow:
                        tankController.Move(Direction.East);
                        break;
                    case ConsoleKey.Spacebar:
                        mainPlayerTank.Shoot(bullets, 5); // Указываем cooldown
                        break;
                }
            }

            // Обновляем врагов
            foreach (var enemy in enemyTanks)
            {
                enemy.Update(bullets);
            }

            // Обновляем снаряды
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.UpdateBullet();

                // Проверяем, попал ли снаряд в врага
                for (int j = enemyTanks.Count - 1; j >= 0; j--)
                {
                    var enemy = enemyTanks[j];
                    if (bullet.BulletX == enemy.X && bullet.BulletY == enemy.Y)
                    {
                        // Удаляем врага из списка
                        enemyTanks.Remove(enemy);
                        bullets.RemoveAt(i); // Удаляем снаряд после попадания
                        break; // Выходим из цикла после попадания
                    }
                }

                if (bullet.BulletX < 0 || bullet.BulletX >= MapWidth || bullet.BulletY < 0 || bullet.BulletY >= MapHeight)
                {
                    bullets.RemoveAt(i); // Удаляем, если снаряд вышел за пределы карты
                }
            }
        }

        public void UpdateTankDirection(Tank playerTank, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    playerTank.Direction = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                    playerTank.Direction = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                    playerTank.Direction = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                    playerTank.Direction = Direction.Right;
                    break;
            }
        }

        private void RenderGame()
        {
            Console.SetCursorPosition(0, 0);
            gameMap.Render(); // Отображаем карту
            renderer.Render(mainPlayerTank, bullets); // Отображаем основной танк и снаряды

            // Отображаем врагов
            foreach (var enemy in enemyTanks)
            {
                renderer.RenderEnemyTank(enemy); // Рендеринг врага
            }
        }
    }
}