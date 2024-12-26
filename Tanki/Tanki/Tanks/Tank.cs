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

        private static readonly Dictionary<Direction, char[][]> TankRepresentations = new Dictionary<Direction, char[][]>
        {
            { Direction.Up, new char[][] { new char[] { '▲', '█' }, new char[] { 'О', '█' } } },
            { Direction.Down, new char[][] { new char[] { 'О', '█' }, new char[] { '▼', '█' } } },
            { Direction.Left, new char[][] { new char[] { '◄', 'О' }, new char[] { '█', '█' } } },
            { Direction.Right, new char[][] { new char[] { 'О', '►' }, new char[] { '█', '█' } } },
        };

        protected Tank(int x, int y, GameMap map)
        {
            X = x;
            Y = y;
            CurrentDirection = Direction.Up;
            gameMap = map;
        }

        public void Move(int dx, int dy)
        {
            if (CanMove(dx, dy))
            {
                X += dx;
                Y += dy;
                CurrentDirection = GetDirection(dx, dy);
            }
        }

        private Direction GetDirection(int dx, int dy) =>
            dy < 0 ? Direction.Up :
            dy > 0 ? Direction.Down :
            dx < 0 ? Direction.Left : Direction.Right;

        public bool CanMove(int dx, int dy)
        {
            int newX = X + dx;
            int newY = Y + dy;

            // Проверяем коллизию для 2x2 танка
            return IsInBounds(newX, newY) &&
                   IsInBounds(newX + 1, newY) &&
                   IsInBounds(newX, newY + 1) &&
                   IsInBounds(newX + 1, newY + 1) &&
                   !IsObstacle(newX, newY) &&
                   !IsObstacle(newX + 1, newY) &&
                   !IsObstacle(newX, newY + 1) &&
                   !IsObstacle(newX + 1, newY + 1);
        }

        private bool IsInBounds(int x, int y) =>
            x >= 0 && x < gameMap.Width && y >= 0 && y < gameMap.Height;

        private bool IsObstacle(int x, int y) =>
            gameMap.GetMapSymbol(x, y) is '#' or 'W' or 'X';

        public Bullet Shoot()
        {
            Bullet bullet = new Bullet(X + 1, Y + 1) { Direction = CurrentDirection }; // Снаряд появляется от центра танка
            return bullet;
        }

        public void ChangeDirection(Direction newDirection) => CurrentDirection = newDirection;

        public void Draw(Renderer renderer)
        {
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