using System;
using System.Collections.Generic;
using Tanki.Map;
using Tanki.Tanks;

namespace Tanki.Tanks.Player
{
    public class PlayerTank : BaseTank<GameState> // Наследуемся от BaseTank с обобщением GameState
    {
        private readonly int shotCooldown; // Время между выстрелами

        public PlayerTank(int x, int y, Direction direction, GameState gameState, int shotCooldown = 5)
            : base(x, y, direction, gameState) // Передаем gameState в базовый класс
        {
            this.shotCooldown = shotCooldown; // Инициализируем cooldown
        }

        public override void Shoot(List<Bullet> bullets, int currentTime)
        {
            if (!IsAlive) return; // Если танк мёртв, не стрелять

            // Проверяем, истек ли cooldown
            if (currentTime % shotCooldown == 0) // Стреляем, если cooldown истек
            {
                CreateBullet(bullets);
            }
        }

        // Реализация абстрактного метода Move
        public override void Move(Direction direction)
        {
            Direction = direction; // Обновляем направление танка
            switch (direction)
            {
                case Direction.Up:
                    Y--; // Двигаем танк вверх
                    break;
                case Direction.Down:
                    Y++; // Двигаем танк вниз
                    break;
                case Direction.Left:
                    X--; // Двигаем танк влево
                    break;
                case Direction.Right:
                    X++; // Двигаем танк вправо
                    break;
            }
            // Дополнительные проверки границ карты могут быть добавлены здесь
        }

        private void CreateBullet(List<Bullet> bullets)
        {
            int bulletX = X;
            int bulletY = Y;

            UpdateBulletPosition(ref bulletX, ref bulletY);

            // Проверка границ карты перед добавлением снаряда
            if (IsBulletInBounds(bulletX, bulletY))
            {
                // Передаем gameState в Bullet
                bullets.Add(new Bullet(bulletX, bulletY, Direction, gameState));
            }
        }

        private void UpdateBulletPosition(ref int bulletX, ref int bulletY)
        {
            switch (Direction)
            {
                case Direction.Up:
                    bulletY--;
                    break;
                case Direction.Down:
                    bulletY++;
                    break;
                case Direction.Left:
                    bulletX--;
                    break;
                case Direction.Right:
                    bulletX++;
                    break;
            }
        }

        private bool IsBulletInBounds(int bulletX, int bulletY)
        {
            return bulletX >= 0 && bulletX < gameState.Map.Width && bulletY >= 0 && bulletY < gameState.Map.Height; // Используем gameState для проверки границ
        }
    }
}