using System;
using System.Collections.Generic;
using Tanki.Map;
using Tanki.Tanks;

namespace Tanki.Tanks.Enemy
{
    public class EnemyTank : Tank
    {
        private Random random;
        private int shootCooldown;
        private int shootTimer;
        private int moveCooldown;
        private int moveTimer;

        public bool IsAlive { get; private set; } // Свойство, указывающее, жив ли враг

        public EnemyTank(int x, int y, Direction direction, GameState gameState, int shotCooldown = 5) // Изменено
            : base(x, y, direction, gameState) // Передаем gameState в базовый класс
        {
            random = new Random();
            shootCooldown = 32; // Количество кадров до следующей стрельбы
            shootTimer = 0;

            moveCooldown = 16; // Количество кадров до следующего движения
            moveTimer = 0;

            IsAlive = true; // Враг считается живым при создании
        }

        public void Update(List<Bullet> bullets)
        {
            if (!IsAlive) return; // Если танк мёртв, ничего не делать

            shootTimer++;
            moveTimer++;

            if (shootTimer >= shootCooldown)
            {
                Shoot(bullets, 5);
                shootTimer = 0; // Сброс таймера после стрельбы
            }

            if (moveTimer >= moveCooldown)
            {
                MoveRandomly();
                moveTimer = 0; // Сброс таймера после движения
            }

            CheckBulletCollisions(bullets);
        }

        private void MoveRandomly()
        {
            Direction newDirection = (Direction)random.Next(0, 4);
            Move(newDirection);
        }

        public override void Shoot(List<Bullet> bullets, int cooldown)
        {
            if (!IsAlive) return; // Если танк мёртв, не стрелять

            Console.WriteLine("EnemyTank shooting at position: " + X + ", " + Y);
            bullets.Add(new Bullet(X, Y, Direction, gameState)); // Передаем gameState
        }

        private void CheckBulletCollisions(List<Bullet> bullets)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = bullets[i];

                if (bullet.BulletX == X && bullet.BulletY == Y)
                {
                    Die();
                    bullets.RemoveAt(i);
                    break;
                }
            }
        }

        private void Die()
        {
            IsAlive = false; // Устанавливаем состояние танка как "мертвый"
        }
    }
}