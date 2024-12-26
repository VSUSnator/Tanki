using System;
using System.Collections.Generic;
using System.Linq;
using Tanki.Map;

namespace Tanki
{
    public class GameState
    {
        private DateTime lastMoveTime;
        private TimeSpan moveDelay = TimeSpan.FromMilliseconds(300); // Задержка между перемещениями
        private DateTime lastShootTime;
        private TimeSpan shootDelay = TimeSpan.FromMilliseconds(900); // Задержка между выстрелами
        private bool isShooting; // Флаг для проверки, нажата ли кнопка стрельбы
        private bool isMoving; // Флаг для проверки, нажата ли кнопка движения

        public Tank PlayerTank { get; private set; }
        public List<Tank> EnemyTanks { get; private set; } // Список врагов
        public List<Bullet> Bullets { get; private set; }
        public GameMap GameMap { get; private set; }
        public bool IsGameActive { get; private set; }

        public GameState(string mapFilePath)
        {
            GameMap = new GameMap(mapFilePath); // Сначала создаем GameMap
            PlayerTank = new TankPlayer(6, 6, GameMap); // Создаем PlayerTank как TankPlayer
            EnemyTanks = new List<Tank>(); // Инициализация списка врагов
            Bullets = new List<Bullet>();
            IsGameActive = false;
            lastMoveTime = DateTime.Now; // Инициализация времени последнего движения
            lastShootTime = DateTime.Now; // Инициализация времени последнего выстрела
        }

        // Метод для добавления врага
        public void AddEnemyTank(int x, int y)
        {
            var enemyTank = new TankEnemy(x, y, GameMap); // Создаем врага как TankEnemy
            EnemyTanks.Add(enemyTank);
        }

        private void TryShoot()
        {
            if (DateTime.Now - lastShootTime >= shootDelay)
            {
                AddBullet();
                lastShootTime = DateTime.Now;
                isShooting = false; // Сбрасываем флаг
            }
        }

        public void TryMove(int dx, int dy)
        {
            if (DateTime.Now - lastMoveTime >= moveDelay) // Проверяем, можно ли двигаться
            {
                PlayerTank.Move(dx, dy); // Движение танка
                lastMoveTime = DateTime.Now; // Обновляем время последнего движения
            }
        }

        public void AddBullet()
        {
            var bullet = PlayerTank.Shoot(); // Правильный вызов метода Shoot
            Bullets.Add(bullet);
        }

        public void StartGame()
        {
            IsGameActive = true;
            // Дополнительная логика инициализации
        }

        public void EndGame()
        {
            IsGameActive = false;
            // Логика завершения, например, сохранение результатов
        }

        public void Update()
        {
            if (IsGameActive)
            {
                // Обработка ввода
                HandleInput();

                // Обновление снарядов
                UpdateBullets();
            }
        }

        public string GetMapSymbol(int x, int y)
        {
            return GameMap.GetMapSymbol(x, y).ToString();
        }

        private bool CheckCollisionWithWall(Bullet bullet)
        {
            return GameMap.IsWall(bullet.X, bullet.Y);
        }

        private bool CheckCollisionWithEnemies(Bullet bullet)
        {
            return EnemyTanks.Any(enemyTank => bullet.X == enemyTank.X && bullet.Y == enemyTank.Y);
        }

        private void UpdateBullets()
        {
            Bullets.RemoveAll(bullet => !bullet.IsInBounds(70, 70)); // Удаляем снаряды, вышедшие за пределы экрана

            foreach (var bullet in Bullets.ToList()) // Используем ToList чтобы избежать изменения коллекции во время итерации
            {
                bullet.Move(); // Двигаем снаряд

                // Проверяем столкновение с объектами
                char symbolAtBulletPosition = GameMap.GetMapSymbol(bullet.X, bullet.Y);
                if (symbolAtBulletPosition == 'X')
                {
                    Console.WriteLine($"Bullet hit at {bullet.X}, {bullet.Y}");
                    GameMap.DestroyObject(bullet.X, bullet.Y); // Уничтожаем объект на позиции снаряда
                    Bullets.Remove(bullet); // Удаляем снаряд
                    continue;
                }

                // Проверка на столкновения с стенами
                if (GameMap.IsWall(bullet.X, bullet.Y))
                {
                    Bullets.Remove(bullet); // Удаляем снаряд при столкновении со стеной
                    continue; // Переход к следующему снаряду
                }

                // Проверяем столкновение с врагами
                foreach (var enemyTank in EnemyTanks)
                {
                    if (bullet.X == enemyTank.X && bullet.Y == enemyTank.Y)
                    {
                        Bullets.Remove(bullet); // Удаляем снаряд при столкновении с врагом
                        break; // Выход из цикла по врагам
                    }
                }
            }
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                // Проверяем нажатие клавиши движения
                if (key == ConsoleKey.W) // Вверх
                {
                    TryMove(0, -1);
                    isMoving = true; // Устанавливаем флаг
                }
                else if (key == ConsoleKey.S) // Вниз
                {
                    TryMove(0, 1);
                    isMoving = true; // Устанавливаем флаг
                }
                else if (key == ConsoleKey.A) // Влево
                {
                    TryMove(-1, 0);
                    isMoving = true; // Устанавливаем флаг
                }
                else if (key == ConsoleKey.D) // Вправо
                {
                    TryMove(1, 0);
                    isMoving = true; // Устанавливаем флаг
                }
                else if (key == ConsoleKey.Spacebar && !isShooting) // Стрельба
                {
                    TryShoot();
                    isShooting = true; // Устанавливаем флаг
                }
            }
            else
            {
                isMoving = false; // Сбрасываем флаг, если нет нажатий
                isShooting = false; // Сбрасываем флаг, если нет нажатий
            }
        }
    }
}