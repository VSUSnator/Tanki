using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tanki.Map;
using static Tanki.Tank;

namespace Tanki
{
    public class GameState
    {
        private ActionDelay moveDelay;
        private ActionDelay shootDelay;
        public Tank PlayerTank { get; private set; }
        public List<Tank> EnemyTanks { get; private set; }
        public List<Bullet> Bullets { get; private set; }
        public GameMap GameMap { get; private set; }
        public bool IsGameActive { get; private set; }
        private List<(int X, int Y)> enemySpawnPositions;
        private Random random;

        public GameState(string mapFilePath)
        {
            GameMap = new GameMap(mapFilePath);
            PlayerTank = new TankPlayer(6, 6, GameMap, this);
            EnemyTanks = new List<Tank>();
            Bullets = new List<Bullet>();
            IsGameActive = false;
            enemySpawnPositions = new List<(int, int)>
            {
                (12, 19), // Первый враг
                (12, 23), // Второй враг
                (14, 14)  // Третий враг, добавленный здесь
            };
            random = new Random();

            InitializeEnemies(4); // Количество врагов
            moveDelay = new ActionDelay(TimeSpan.FromMilliseconds(300));
            shootDelay = new ActionDelay(TimeSpan.FromMilliseconds(900));
        }

        private void InitializeEnemies(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var spawnPosition = enemySpawnPositions[random.Next(enemySpawnPositions.Count)];
                EnemyTanks.Add(new TankEnemy(spawnPosition.X, spawnPosition.Y, GameMap, this));
            }
        }

        public List<Tank> GetAllTanks()
        {
            var allTanks = new List<Tank> { PlayerTank }; // Начинаем со списка, содержащего игрока
            allTanks.AddRange(EnemyTanks); // Добавляем врагов
            return allTanks; // Возвращаем общий список танков
        }

        public void StartGame() => IsGameActive = true;

        public void TryMove(int dx, int dy)
        {
            if (moveDelay.CanPerformAction())
            {
                PlayerTank.Move(dx, dy);
                moveDelay.Reset();
            }
        }

        public void TryShoot()
        {
            if (shootDelay.CanPerformAction())
            {
                AddBullet();
                shootDelay.Reset();
            }
        }

        public bool AreAllEnemiesDefeated()
        {
            return !EnemyTanks.Any(); // Возвращает true, если список врагов пуст
        }

        private void AddBullet()
        {
            Bullet bullet = PlayerTank.Shoot();
            if (bullet != null)
            {
                Bullets.Add(bullet);
            }
        }

        private void UpdateEnemies()
        {
            for (int i = EnemyTanks.Count - 1; i >= 0; i--)
            {
                if (EnemyTanks[i].IsDestroyed)
                {
                    Console.WriteLine($"Удаление танка врага на позиции ({EnemyTanks[i].X}, {EnemyTanks[i].Y})");
                    EnemyTanks.RemoveAt(i); // Удаляем уничтоженный танк
                }
            }
        }

        // Вызывайте UpdateEnemies в методе Update
        public void Update(TimeSpan gameTime)
        {
            if (IsGameActive)
            {
                (PlayerTank as TankPlayer)?.HandleInput();
                UpdateEnemies();
                foreach (var enemy in EnemyTanks.OfType<TankEnemy>())
                {
                    enemy.Update(gameTime);
                }
                UpdateBullets();
                CheckPlayerHitByBullets(); // Проверяем попадания пуль в игрока
            }
        }

        private void UpdateBullets()
        {
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                var bullet = Bullets[i];
                if (bullet == null || !bullet.IsInBounds(GameMap.Width, GameMap.Height))
                {
                    Bullets.RemoveAt(i);
                }
                else
                {
                    bullet.Move();
                    HandleBulletCollision(bullet);
                }
            }
        }

        private void HandleBulletCollision(Bullet bullet)
        {
            if (GameMap.IsWall(bullet.X, bullet.Y))
            {
                HandleWallDestruction(bullet);
                Bullets.Remove(bullet);
                return;
            }

            if (CheckCollisionWithEnemies(bullet) || PlayerTank.IsHitByBullet(bullet))
            {
                Bullets.Remove(bullet);
            }
        }

        public void CheckPlayerHitByBullets()
        {
            foreach (var bullet in Bullets.ToList())
            {
                if (PlayerTank.IsHitByBullet(bullet))
                {
                    Console.WriteLine("Попадание в игрока!");
                    (PlayerTank as TankPlayer)?.TakeDamage(); // Теперь вызывает метод у TankPlayer
                    bullet.Deactivate(); // Деактивируем пулю после попадания
                }
            }
        }

        public void EndGame()
        {
            IsGameActive = false;
            // Логика завершения игры, например, сохранение результатов или очистка состояния игры
        }

        private bool CheckCollisionWithEnemies(Bullet bullet)
        {
            var hitEnemies = EnemyTanks.Where(enemyTank => enemyTank.IsHitByBullet(bullet)).ToList();

            foreach (var enemy in hitEnemies)
            {
                EnemyTanks.Remove(enemy);
                // Дополнительная логика для обработки уничтоженного врага
            }

            return hitEnemies.Count > 0 || PlayerTank.IsHitByBullet(bullet);
        }

        private void HandleWallDestruction(Bullet bullet)
        {
            int wallsDestroyed = 0;
            char wallSymbol = GameMap.GetMapSymbol(bullet.X, bullet.Y);

            if (wallSymbol == 'X')
            {
                GameMap.DestroyObject(bullet.X, bullet.Y);
                wallsDestroyed++;

                foreach (var offset in DeterminePerpendicularOffsets(bullet.Direction))
                {
                    if (wallsDestroyed >= 2) break;

                    int nextX = bullet.X + offset.Item1;
                    int nextY = bullet.Y + offset.Item2;
                    if (nextX >= 0 && nextX < GameMap.Width && nextY >= 0 && nextY < GameMap.Height)
                    {
                        char nextWallSymbol = GameMap.GetMapSymbol(nextX, nextY);
                        if (nextWallSymbol == 'X')
                        {
                            GameMap.DestroyObject(nextX, nextY);
                            wallsDestroyed++;
                        }
                    }
                }
            }
        }

        private static (int, int)[] DeterminePerpendicularOffsets(Direction direction)
        {
            return direction switch
            {
                Direction.Up or Direction.Down => new[] { (-1, 0), (1, 0) },
                Direction.Left or Direction.Right => new[] { (0, -1), (0, 1) },
                _ => throw new InvalidOperationException("Неизвестное направление"),
            };
        }
    }
}