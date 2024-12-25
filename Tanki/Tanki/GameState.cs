using System.Collections.Generic;
using Tanki.Map;

namespace Tanki
{
    public class GameState
    {
        public Tank PlayerTank { get; private set; }
        public List<Tank> EnemyTanks { get; private set; } // Список врагов
        public List<Bullet> Bullets { get; private set; }
        public GameMap GameMap { get; private set; }
        public bool IsGameActive { get; private set; }

        public GameState(string mapFilePath)
        {
            GameMap = new GameMap(mapFilePath); // Сначала создаем GameMap
            PlayerTank = new Tank(1, 1, GameMap); // Теперь инициализируем PlayerTank с уже созданной GameMap
            EnemyTanks = new List<Tank>(); // Инициализация списка врагов
            Bullets = new List<Bullet>();
            IsGameActive = false;
        }

        // Метод для добавления врага
        public void AddEnemyTank(int x, int y)
        {
            var enemyTank = new Tank(x, y, GameMap);
            EnemyTanks.Add(enemyTank);
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
            Bullets.RemoveAll(bullet => !bullet.IsInBounds(20, 15)); // Удаляем снаряды, вышедшие за пределы экрана

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
    }   
}