using System;
using Tanki.Map;
using Tanki.Tanks;

namespace Tanki
{
    public class Bullet
    {
        public int BulletX { get; private set; }
        public int BulletY { get; private set; }
        public Direction Direction { get; private set; }
        private GameState gameState;

        public Bullet(int x, int y, Direction direction, GameState gameState) // Изменено здесь
        {
            BulletX = x;
            BulletY = y;
            Direction = direction;
            this.gameState = gameState ?? throw new InvalidOperationException("GameState is not initialized."); // Проверка на инициализацию
        }

        public bool UpdateBullet(Action<int, int> onHitEnemy)
        {
            if (!IsInBounds() || !gameState.Map.CanShootThrough(BulletX, BulletY))
            {
                return false; // Удаляем пулю, если она выходит за пределы карты или не может пройти
            }

            if (gameState.Map.HasEnemyAt(BulletX, BulletY))
            {
                onHitEnemy(BulletX, BulletY);
                return false; // Удаляем пулю после попадания
            }

            Move();
            return true; // Пуля продолжает движение
        }

        private void Move()
        {
            switch (Direction)
            {
                case Direction.Up:
                case Direction.North:
                    BulletY--;
                    break;
                case Direction.Down:
                case Direction.South:
                    BulletY++;
                    break;
                case Direction.Right:
                case Direction.East:
                    BulletX++;
                    break;
                case Direction.Left:
                case Direction.West:
                    BulletX--;
                    break;
            }
        }

        private bool IsInBounds()
        {
            return BulletX >= 0 && BulletX < gameState.Map.Width &&
                   BulletY >= 0 && BulletY < gameState.Map.Height;
        }
    }
}