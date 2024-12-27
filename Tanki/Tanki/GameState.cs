﻿using System;
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
        private bool isShooting; // Флаг для проверки, нажата ли кнопка стрельбы

        public Tank PlayerTank { get; private set; }
        public List<Tank> EnemyTanks { get; private set; } // Список врагов
        public List<Bullet> Bullets { get; private set; }
        public GameMap GameMap { get; private set; }
        public bool IsGameActive { get; private set; }
        public List<TankEnemy> Enemies { get; set; }

        public GameState(string mapFilePath)
        {
            GameMap = new GameMap(mapFilePath); // Создаем GameMap
            PlayerTank = new TankPlayer(6, 6, GameMap, this); // Передаем GameState в конструктор TankPlayer
            EnemyTanks = new List<Tank>(); // Инициализация списка врагов
            Enemies = new List<TankEnemy>();
            Bullets = new List<Bullet>();
            IsGameActive = false;

            // Инициализация задержек
            moveDelay = new ActionDelay(TimeSpan.FromMilliseconds(300));
            shootDelay = new ActionDelay(TimeSpan.FromMilliseconds(900));
        }

        // Метод для добавления врага
        public void AddEnemyTank(int x, int y)
        {
            var enemyTank = new TankEnemy(x, y, GameMap); // Создаем врага как TankEnemy
            EnemyTanks.Add(enemyTank);
        }

        public void InitializeEnemies()
        {
            // Пример создания врагов на карте
            Enemies.Add(new TankEnemy(5, 5, GameMap));
            Enemies.Add(new TankEnemy(10, 10, GameMap));
        }

        public void StartGame()
        {
            IsGameActive = true;
            // Дополнительная логика инициализации
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
            var hitEnemies = EnemyTanks.Where(enemyTank => enemyTank.IsHit(bullet.X, bullet.Y)).ToList();
            foreach (var enemyTank in hitEnemies)
            {
                EnemyTanks.Remove(enemyTank); // Удаляем врага (при необходимости)
            }
            return hitEnemies.Count > 0; // Возвращаем true, если есть столкновение
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
                // Обработка ввода для игрока
                // Мы вызываем HandleInput только для PlayerTank, который является TankPlayer
                if (PlayerTank is TankPlayer playerTank)
                {
                    playerTank.HandleInput(); // Обработка ввода для игрока
                }
                foreach (var enemy in Enemies)
                {
                    enemy.Update(this); // Обновляем каждого врага
                }
                // Обновление снарядов
                UpdateBullets();
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
                    // Логика разрушения стен
                    HandleWallDestruction(bullet);

                    Bullets.Remove(bullet); // Удаляем снаряд после проверки
                    continue;
                }

                // Проверка на столкновение с врагами
                if (CheckCollisionWithEnemies(bullet))
                {
                    // Логика обработки столкновения с врагами
                    Bullets.Remove(bullet); // Удаляем пулю, если она попала в врага
                    continue;
                }
            }
        }

        public void Draw(Renderer renderer)
        {
            PlayerTank?.Draw(renderer); // Отрисовка игрока

            foreach (var enemy in Enemies)
            {
                enemy.Draw(renderer); // Отрисовка врагов
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