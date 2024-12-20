using System;
using Tanki.Map;

namespace Tanki.Tanks
{
    public class Bullet
    {
        public int BulletX { get; private set; }
        public int BulletY { get; private set; }
        public Direction Direction { get; private set; }
        private GameMap gameMap; // Ссылка на карту
        private int updateCooldown; // Количество кадров между обновлениями

        // Конструктор с пятью параметрами
        public Bullet(int x, int y, Direction direction, GameMap map, int cooldown = 5)
        {
            BulletX = x;
            BulletY = y;
            Direction = direction;
            gameMap = map; // Инициализируем карту
            updateCooldown = cooldown; // Устанавливаем значение cooldown
        }

        public bool UpdateBullet(Action<int, int> onHitEnemy)
        {
            if (gameMap == null)
            {
                throw new InvalidOperationException("GameMap is not initialized."); // Исключение, если gameMap не инициализирован
            }

            // Проверяем, можно ли двигаться дальше
            if (BulletX < 0 || BulletX >= gameMap.Width ||
                BulletY < 0 || BulletY >= gameMap.Height ||
                !gameMap.CanShootThrough(BulletX, BulletY))
            {
                return false; // Если снаряд не может пройти, возвращаем false для удаления
            }

            // Проверяем столкновение с врагами
            if (gameMap.HasEnemyAt(BulletX, BulletY))
            {
                // Уведомляем о попадании во врага
                onHitEnemy(BulletX, BulletY);
                return false; // Удаляем пулю
            }

            // Обновляем позицию снаряда
            MoveBullet();

            return true; // Если снаряд продолжает движение, возвращаем true
        }

        private void MoveBullet()
        {
            switch (Direction)
            {
                case Direction.Up:
                case Direction.North:
                    BulletY--; // Двигаем снаряд вверх
                    break;
                case Direction.Down:
                case Direction.South:
                    BulletY++; // Двигаем снаряд вниз
                    break;
                case Direction.Right:
                case Direction.East:
                    BulletX++; // Двигаем снаряд вправо
                    break;
                case Direction.Left:
                case Direction.West:
                    BulletX--; // Двигаем снаряд влево
                    break;
            }
        }
    }
}