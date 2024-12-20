using System;
using System.Collections.Generic;
using Tanki.Map;
using Tanki.Tanks;
using Tanki.Tanks.Enemy;
using Tanki.Tanks.Player;

namespace Tanki
{
    public class Game
    {
        private PlayerTank mainPlayerTank;
        private List<EnemyTank> enemyTanks;
        private Renderer renderer;
        private List<Bullet> bullets = new List<Bullet>();
        public static int MapWidth { get; private set; } = 50;
        public static int MapHeight { get; private set; } = 24;
        private GameMap gameMap;
        private TankMovement tankMovement;
        private InputHandler inputHandler; // Добавляем InputHandler
        private int frameCount = 0;
        private const int FrameRenderRate = 4;
        private const int FrameDelay = 100;

        public Game()
        {
            gameMap = new GameMap(MapWidth, MapHeight);
            mainPlayerTank = new PlayerTank(MapWidth / 8, MapHeight / 7, Direction.Up);
            enemyTanks = new List<EnemyTank>
            {
                new EnemyTank(MapWidth / 10, MapHeight / 4, Direction.Right, gameMap),
                new EnemyTank(MapWidth / 3, MapHeight / 3, Direction.Left, gameMap)
            };
            renderer = new Renderer(MapWidth, MapHeight);
            tankMovement = new TankMovement(mainPlayerTank, gameMap);
            inputHandler = new InputHandler(mainPlayerTank, tankMovement); // Инициализируем InputHandler

            // Подписываемся на событие OnShoot
            inputHandler.OnShoot += mainPlayerTank.Shoot;
        }

        public void Start()
        {
            DateTime lastFrameTime = DateTime.Now;

            while (true)
            {
                UpdateGameState();

                if (frameCount % FrameRenderRate == 0)
                {
                    RenderGame();
                }

                frameCount++;
                MaintainFrameRate(lastFrameTime);
                lastFrameTime = DateTime.Now;
            }
        }

        private void UpdateGameState()
        {
            inputHandler.ProcessInput(bullets, frameCount); // Передаем снаряды и кадры для стрельбы

            foreach (var enemy in enemyTanks)
            {
                enemy.Update(bullets);
            }

            UpdateBullets();
        }

        private void UpdateBullets()
        {
            bullets.RemoveAll(b => !b.UpdateBullet(OnBulletHit) || !IsBulletInBounds(b));

            var bulletsToRemove = new HashSet<int>();
            var enemiesToRemove = new HashSet<int>();

            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < enemyTanks.Count; j++)
                {
                    if (bullets[i].BulletX == enemyTanks[j].X && bullets[i].BulletY == enemyTanks[j].Y)
                    {
                        enemiesToRemove.Add(j);
                        bulletsToRemove.Add(i);
                        break;
                    }
                }
            }

            foreach (var index in enemiesToRemove)
            {
                enemyTanks.RemoveAt(index);
            }

            foreach (var index in bulletsToRemove)
            {
                bullets.RemoveAt(index);
            }
        }

        // Метод, который будет вызываться при попадании снаряда
        private void OnBulletHit(int x, int y)
        {
            // Здесь можно добавить логику обработки попадания, например, удаления врага
            for (int i = 0; i < enemyTanks.Count; i++)
            {
                if (enemyTanks[i].X == x && enemyTanks[i].Y == y)
                {
                    enemyTanks.RemoveAt(i);
                    break; // Выход, если враг найден и удален
                }
            }
        }

        private bool IsBulletInBounds(Bullet bullet)
        {
            return bullet.BulletX >= 0 && bullet.BulletX < MapWidth &&
                   bullet.BulletY >= 0 && bullet.BulletY < MapHeight;
        }

        private void RenderGame()
        {
            Console.SetCursorPosition(0, 0);
            gameMap.Render();
            renderer.Render(mainPlayerTank, bullets);

            foreach (var enemy in enemyTanks)
            {
                renderer.RenderEnemyTank(enemy);
            }
        }

        private void MaintainFrameRate(DateTime lastFrameTime)
        {
            TimeSpan elapsedTime = DateTime.Now - lastFrameTime;

            if (elapsedTime.TotalMilliseconds < FrameDelay)
            {
                System.Threading.Thread.Sleep(FrameDelay - (int)elapsedTime.TotalMilliseconds);
            }
        }
    }
}