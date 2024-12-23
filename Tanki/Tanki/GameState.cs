using System;
using System.Collections.Generic;
using System.Linq;
using Tanki.Map;
using Tanki.Tanks.Enemy;

namespace Tanki
{
    public class GameState
    {
        private List<Bullet> bullets;
        private List<EnemyTank> enemyTanks;
        public GameMap Map { get; private set; } // Добавляем свойство для GameMap

        public GameState(GameMap map)
        {
            Map = map;
            bullets = new List<Bullet>();
            enemyTanks = new List<EnemyTank>();
        }

        public IReadOnlyList<Bullet> Bullets => bullets.AsReadOnly();
        public IReadOnlyList<EnemyTank> EnemyTanks => enemyTanks.AsReadOnly();

        public void Update()
        {
            UpdateBullets();
            // Здесь можно добавить обновление состояния врагов
        }

        private void UpdateBullets()
        {
            var bulletsToRemove = new List<Bullet>();
            var enemiesToRemove = new List<EnemyTank>();

            foreach (var bullet in bullets)
            {
                if (!bullet.UpdateBullet(OnBulletHit) || !IsBulletInBounds(bullet))
                {
                    bulletsToRemove.Add(bullet);
                    continue;
                }
                CheckBulletCollisionWithEnemies(bullet, enemiesToRemove, bulletsToRemove);
            }

            RemoveObjects(bullets, bulletsToRemove);
            RemoveObjects(enemyTanks, enemiesToRemove);
        }

        public void AddBullet(Bullet bullet)
        {
            bullets.Add(bullet);
        }

        public void AddEnemyTanks(IEnumerable<EnemyTank> enemies)
        {
            enemyTanks.AddRange(enemies);
        }

        private void CheckBulletCollisionWithEnemies(Bullet bullet, List<EnemyTank> enemiesToRemove, List<Bullet> bulletsToRemove)
        {
            foreach (var enemy in enemyTanks)
            {
                if (bullet.BulletX == enemy.X && bullet.BulletY == enemy.Y)
                {
                    enemiesToRemove.Add(enemy);
                    bulletsToRemove.Add(bullet);
                    break; // Выход, если снаряд попал в врага
                }
            }
        }

        private void RemoveObjects<T>(List<T> list, List<T> objectsToRemove)
        {
            foreach (var obj in objectsToRemove)
            {
                list.Remove(obj);
            }
        }

        private bool IsBulletInBounds(Bullet bullet)
        {
            return bullet.BulletX >= 0 && bullet.BulletX < Map.Width &&
                   bullet.BulletY >= 0 && bullet.BulletY < Map.Height;
        }

        private void OnBulletHit(int x, int y)
        {
            // Логика для разрушения блока, если это возможно
            if (Map.CanMoveTo(x, y)) // Проверяем, что в этой позиции можно разрушить блок
            {
                Map.DestroyBlock(x, y); // Разрушаем блок
            }
            else // Проверяем, есть ли враг в этой позиции
            {
                enemyTanks.RemoveAll(enemy => enemy.X == x && enemy.Y == y);
            }
        }
    }
}