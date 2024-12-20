using System;
using System.Collections.Generic;
using Tanki.Tanks;
using Tanki.Map;

namespace Tanki.Tanks.Player
{
    public class PlayerTank : Tank
    {
        private int shotCooldown; // Время между выстрелами
        private GameMap gameMap;

        public PlayerTank(int x, int y, Direction direction, int shotCooldown = 5, GameMap? map = null)
            : base(x, y, direction)
        {
            this.shotCooldown = shotCooldown;
            gameMap = map; // Теперь map может быть null
        }

        public override void Shoot(List<Bullet> bullets, int currentTime)
        {
            if (!state.IsAlive) return; // Если танк мёртв, не стрелять

            // Проверяем, истек ли cooldown
            if (currentTime % shotCooldown == 0) // Стреляем, если cooldown истек
            {
                CreateBullet(bullets);
            }
        }

        private void CreateBullet(List<Bullet> bullets)
        {
            int bulletX = X;
            int bulletY = Y;

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

            // Проверка границ карты перед добавлением снаряда
            if (gameMap != null && bulletX >= 0 && bulletX < Game.MapWidth && bulletY >= 0 && bulletY < Game.MapHeight)
            {
                // Создаем новый снаряд с заданным cooldown
                bullets.Add(new Bullet(bulletX, bulletY, Direction, gameMap, shotCooldown));
            }
        }
    }
}