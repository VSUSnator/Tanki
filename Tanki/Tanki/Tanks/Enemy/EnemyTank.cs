using System;
using System.Collections.Generic;

namespace Tanki.Tanks.Enemy
{
    public class EnemyTank : Tank
    {
        private Random random;
        private int shootCooldown;
        private int shootTimer;
        private int moveCooldown;
        private int moveTimer;

        public EnemyTank(int x, int y, Direction direction) : base(x, y, direction)
        {
            random = new Random();
            shootCooldown = 32; // Количество кадров до следующей стрельбы
            shootTimer = 0;

            moveCooldown = 16; // Количество кадров до следующего движения
            moveTimer = 0;
        }

        public void Update(List<Bullet> bullets)
        {
            // Увеличиваем таймеры
            shootTimer++;
            moveTimer++;

            // Логика стрельбы
            if (shootTimer >= shootCooldown)
            {
                Shoot(bullets, 5); // Передаем значение cooldown, например, 5
                shootTimer = 0; // Сброс таймера после стрельбы
            }

            // Логика движения
            if (moveTimer >= moveCooldown)
            {
                MoveRandomly();
                moveTimer = 0; // Сброс таймера после движения
            }
        }

        private void MoveRandomly()
        {
            // Случайное направление
            Direction newDirection = (Direction)random.Next(0, 4); // 0-3 для 4 направлений
            Direction = newDirection;

            // Двигаем танк в случайном направлении
            Move(newDirection);
        }

        public override void Shoot(List<Bullet> bullets, int cooldown) // Обновленная сигнатура метода
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

            bullets.Add(new Bullet(bulletX, bulletY, Direction, cooldown));
        }
    }
}