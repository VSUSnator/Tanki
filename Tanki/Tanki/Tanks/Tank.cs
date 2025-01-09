using System.Collections.Generic;
using Tanki.Map;

namespace Tanki
{
    public abstract class Tank : ITank
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public int X { get; protected set; }
        public int Y { get; protected set; }
        public Direction CurrentDirection { get; protected set; }
        protected readonly GameMap gameMap;
        protected int lives;
        public bool IsDestroyed { get; private set; }

        protected static readonly Dictionary<Direction, char[][]> TankRepresentations = new()
        {
            { Direction.Up, new char[][] { new char[] { '▲', '█' }, new char[] { 'О', '█' } } },
            { Direction.Down, new char[][] { new char[] { 'О', '█' }, new char[] { '▼', '█' } } },
            { Direction.Left, new char[][] { new char[] { '◄', 'О' }, new char[] { '█', '█' } } },
            { Direction.Right, new char[][] { new char[] { 'О', '►' }, new char[] { '█', '█' } } },
        };

        private readonly ActionDelay moveDelay = new(TimeSpan.FromMilliseconds(300));
        private readonly ActionDelay shootDelay = new(TimeSpan.FromMilliseconds(900));

        protected Tank(int x, int y, GameMap map)
        {
            X = x;
            Y = y;
            CurrentDirection = Direction.Up;
            gameMap = map;
            lives = 1;
        }

        public void Move(int dx, int dy)
        {
            // Проверяем возможность движения, включая коллизии
            if (moveDelay.CanPerformAction() && CanMove(dx, dy))
            {
                // Обновляем позицию
                X += dx;
                Y += dy;
                CurrentDirection = GetDirection(dx, dy);
                moveDelay.Reset();
            }
        }

        public bool CanMove(int dx, int dy)
        {
            int newX = X + dx;
            int newY = Y + dy;

            // Проверяем, находится ли новая позиция в пределах карты
            if (!IsInBounds(newX, newY) || !IsInBounds(newX + 1, newY) ||
                !IsInBounds(newX, newY + 1) || !IsInBounds(newX + 1, newY + 1))
            {
                return false; // Выход за пределы карты
            }

            // Проверяем, есть ли препятствия на новой позиции
            if (IsObstacle(newX, newY) ||
                IsObstacle(newX + 1, newY) ||
                IsObstacle(newX, newY + 1) ||
                IsObstacle(newX + 1, newY + 1))
            {
                return false; // Есть препятствия
            }

            // Проверяем коллизии с другими танками
            foreach (var enemyTank in gameMap.GetAllTanks())
            {
                if (enemyTank != this && enemyTank.IsCollidingWithTank(newX, newY))
                {
                    return false; // Обнаружена коллизия с другим танком
                }
            }

            return true; // Движение возможно
        }

        public bool IsCollidingWithTank(int x, int y)
        {
            // Проверка на пересечение с другим танком
            return x >= X && x <= X + 1 && y >= Y && y <= Y + 1;
        }

        public Bullet? Shoot()
        {
            if (!shootDelay.CanPerformAction()) return null;

            Bullet? bullet = CreateBullet();
            bullet.Direction = CurrentDirection;
            shootDelay.Reset();
            return bullet;
        }

        private Bullet? CreateBullet()
        {
            return CurrentDirection switch
            {
                Direction.Up => new Bullet(X + 1, Y),
                Direction.Down => new Bullet(X + 1, Y + 1),
                Direction.Left => new Bullet(X, Y + 1),
                Direction.Right => new Bullet(X + 1, Y + 1),
                _ => null
            };
        }

        public bool IsHit(int x, int y)
        {
            return this.X == x && this.Y == y;
        }

        private Direction GetDirection(int dx, int dy) =>
            dy < 0 ? Direction.Up :
            dy > 0 ? Direction.Down :
            dx < 0 ? Direction.Left : Direction.Right;

        private bool IsInBounds(int x, int y) =>
            x >= 0 && x < gameMap.Width && y >= 0 && y < gameMap.Height;

        protected bool IsObstacle(int x, int y) =>
            gameMap.GetMapSymbol(x, y) is '#' or 'W' or 'X';

        public void ChangeDirection(Direction newDirection) => CurrentDirection = newDirection;

        public void TakeDamage()
        {
            if (IsDestroyed) return; // Если танк уже уничтожен, ничего не делаем

            lives--; // Уменьшаем количество жизней
            if (lives <= 0)
            {
                IsDestroyed = true; // Устанавливаем состояние на "уничтожен"
            }
        }

        public bool IsHitByBullet(Bullet bullet)
        {
            bool isHit = bullet.X >= X && bullet.X <= X + 1 &&
                         bullet.Y >= Y && bullet.Y <= Y + 1;

            if (isHit)
            {
                TakeDamage(); // Если танк был поражен, вызываем метод повреждения
            }

            return isHit;
        }

        public virtual void Draw(Renderer renderer)
        {
            if (IsDestroyed)
            {
                renderer.SetPixel(X, Y, '✖', 2); // Отображаем символ "X" на позиции танка
                return;
            }

            char[][] tankRepresentation = TankRepresentations[CurrentDirection];
            for (int i = 0; i < tankRepresentation.Length; i++)
            {
                for (int j = 0; j < tankRepresentation[i].Length; j++)
                {
                    renderer.SetPixel(X + j, Y + i, tankRepresentation[i][j], 2);
                }
            }
        }
    }
}