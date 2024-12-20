using System;
using System.Collections.Generic;
using Tanki.Map;

namespace Tanki.Tanks.Enemy
{
    public class EnemyTank : Tank
    {
        private Random random;
        private int shootCooldown;
        private int shootTimer;
        private int moveCooldown;
        private int moveTimer;
        private GameMap gameMap;

        public bool IsAlive { get; private set; } // Свойство, указывающее, жив ли враг

        public EnemyTank(int x, int y, Direction direction, GameMap map) : base(x, y, direction)
        {
            random = new Random();
            shootCooldown = 32; // Количество кадров до следующей стрельбы
            shootTimer = 0;

            moveCooldown = 16; // Количество кадров до следующего движения
            moveTimer = 0;

            gameMap = map;
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

            base.Shoot(bullets, cooldown);
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