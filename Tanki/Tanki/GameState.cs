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
        private ActionDelay moveDelay; // Задержка между перемещениями
        private ActionDelay shootDelay; // Задержка между выстрелами

        public Tank PlayerTank { get; private set; }
        public List<Tank> EnemyTanks { get; private set; } // Список врагов
        public List<Bullet> Bullets { get; private set; }
        public GameMap GameMap { get; private set; }
        public bool IsGameActive { get; private set; }

        // Предопределенные позиции для врагов
        private List<(int X, int Y)> enemySpawnPositions = new List<(int, int)>
        {
            /*(1, 1), (2, 1),*/ (18, 15) // Примеры координат
            /*(1, 3), (2, 3),*/ 
            /*(5, 5), (6, 5),*/ 
            // Добавьте здесь другие координаты по желанию
        };

        public GameState(string mapFilePath)
        {
            GameMap = new GameMap(mapFilePath);
            PlayerTank = new TankPlayer(6, 6, GameMap, this);
            EnemyTanks = new List<Tank>();
            Bullets = new List<Bullet>();
            IsGameActive = false;

            // Создаем нескольких врагов
            for (int i = 0; i < 2; i++) // Например, 2 врага
            {
                // Выбираем случайную позицию из списка заранее заданных
                var spawnPosition = enemySpawnPositions[new Random().Next(enemySpawnPositions.Count)];
                EnemyTanks.Add(new TankEnemy(spawnPosition.X, spawnPosition.Y, GameMap, this)); // Передаем gameState
            }

            // Инициализация задержек
            moveDelay = new ActionDelay(TimeSpan.FromMilliseconds(300));
            shootDelay = new ActionDelay(TimeSpan.FromMilliseconds(900));
        }

        public void StartGame()
        {
            IsGameActive = true;
        }

        public void TryMove(int dx, int dy)
        {
            if (moveDelay.CanPerformAction()) // Проверяем, можно ли двигаться
            {
                PlayerTank.Move(dx, dy); // Движение танка
                moveDelay.Reset(); // Обновляем время последнего движения
            }
        }

        public void TryShoot()
        {
            if (shootDelay.CanPerformAction())
            {
                AddBullet();
                shootDelay.Reset(); // Сбрасываем таймер стрельбы
            }
        }

        private void AddBullet()
        {
            Bullet bullet = PlayerTank.Shoot(); // Используем метод Shoot для создания снаряда
            if (bullet != null)
            {
                Bullets.Add(bullet); // Добавляем пулю в список, если она не null
            }
        }

        private bool CheckCollisionWithEnemies(Bullet bullet)
        {
            var hitEnemies = EnemyTanks.Where(enemyTank => enemyTank.IsHitByBullet(bullet)).ToList();

            foreach (var enemyTank in hitEnemies)
            {
                /*EnemyTanks.Remove(enemyTank);*/ // Удаляем врага (при необходимости)
            }

            return hitEnemies.Count > 0; // Возвращаем true, если есть столкновение
        }

        public void EndGame()
        {
            IsGameActive = false;
            // Логика завершения, например, сохранение результатов
        }

        public void Update(TimeSpan gameTime)
        {
            if (IsGameActive)
            {
                // Обработка ввода для игрока
                if (PlayerTank is TankPlayer playerTank)
                {
                    playerTank.HandleInput();
                }

                // Обновление врагов
                foreach (var enemy in EnemyTanks)
                {
                    if (enemy is TankEnemy tankEnemy)
                    {
                        // Передаем gameTime в метод Update врага
                        tankEnemy.Update(gameTime); // Теперь передаем аргумент
                    }
                }

                // Обновление снарядов
                UpdateBullets();
            }
        }

        public List<Tank> GetAllTanks()
        {
            // Предполагается, что у вас есть список всех танков в состоянии игры
            return new List<Tank> { PlayerTank /* , другие танки */ };
        }

        private void MoveEnemyTowardsPlayer(TankEnemy enemy)
        {
            int dx = PlayerTank.X - enemy.X;
            int dy = PlayerTank.Y - enemy.Y;

            // Нормализуем направление движения
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                dx = dx > 0 ? 1 : -1; // Движение по оси X
                dy = 0; // Не двигаться по оси Y
            }
            else
            {
                dy = dy > 0 ? 1 : -1; // Движение по оси Y
                dx = 0; // Не двигаться по оси X
            }

            // Проверка коллизий перед перемещением
            if (!GameMap.IsWall(enemy.X + dx, enemy.Y + dy) &&
                !(PlayerTank.X == enemy.X + dx && PlayerTank.Y == enemy.Y + dy)) // Проверка на столкновение с игроком
            {
                enemy.Move(dx, dy); // Движение врага
            }
        }

        private void UpdateBullets()
        {
            // Удаление пуль, которые вышли за границы карты или равны null
            Bullets.RemoveAll(bullet => bullet == null || !bullet.IsInBounds(GameMap.Width, GameMap.Height));

            foreach (var bullet in Bullets.ToList())
            {
                bullet.Move();

                // Проверяем, является ли текущая позиция пулей стеной
                if (GameMap.IsWall(bullet.X, bullet.Y))
                {
                    HandleWallDestruction(bullet);
                    Bullets.Remove(bullet); // Удаляем снаряд после проверки
                    continue;
                }

                // Проверка на столкновение с врагами
                if (CheckCollisionWithEnemies(bullet))
                {
                    Bullets.Remove(bullet); // Удаляем пулю, если она попала в врага
                    continue;
                }

                // Проверка на столкновение с игроком, если у вас есть игрок
                if (PlayerTank.IsHitByBullet(bullet))
                {
                    Bullets.Remove(bullet); // Удаляем пулю, если она попала в игрока
                                            // Здесь можно добавить логику, например, уменьшение здоровья игрока
                }
            }
        }

        private void HandleWallDestruction(Bullet bullet)
        {
            int wallsDestroyed = 0; // Счетчик разрушенных стен
            char wallSymbol = GameMap.GetMapSymbol(bullet.X, bullet.Y);

            // Убедимся, что это разрушимая стена
            if (wallSymbol == 'X') // Разрушаем только стену 'X'
            {
                GameMap.DestroyObject(bullet.X, bullet.Y); // Разрушаем первую стену
                wallsDestroyed++;

                // Определяем направление, перпендикулярное движению пули
                var perpendicularDirections = DeterminePerpendicularOffsets(bullet.Direction);

                // Проверяем обе стороны перпендикулярно направлению движения
                foreach (var offset in perpendicularDirections)
                {
                    if (wallsDestroyed >= 2) // Если разрушено уже 2 стены, выходим из цикла
                    {
                        break;
                    }

                    int nextX = bullet.X + offset.Item1;
                    int nextY = bullet.Y + offset.Item2;

                    if (nextX >= 0 && nextX < GameMap.Width &&
                        nextY >= 0 && nextY < GameMap.Height)
                    {
                        char nextWallSymbol = GameMap.GetMapSymbol(nextX, nextY);

                        // Убедимся, что это разрушимая стена
                        if (nextWallSymbol == 'X')
                        {
                            GameMap.DestroyObject(nextX, nextY); // Разрушаем вторую стену
                            wallsDestroyed++;
                        }
                    }
                }
            }
        }

        // Функция для определения смещений, перпендикулярных заданному направлению
        private static (int, int)[] DeterminePerpendicularOffsets(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                case Direction.Down:
                    return new[] { (-1, 0), (1, 0) }; // Смещения для вертикального движения
                case Direction.Left:
                case Direction.Right:
                    return new[] { (0, -1), (0, 1) }; // Смещения для горизонтального движения
                default:
                    throw new InvalidOperationException("Неизвестное направление");
            }
        }
    }
}