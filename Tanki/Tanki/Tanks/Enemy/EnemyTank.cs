using Tanki.Map;

namespace Tanki
{
    public class Obstacle
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Obstacle(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Enemy : IMovable
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Enemy(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move(int dx, int dy)
        {
            // Проверка границ экрана
            if (X + dx >= 0 && X + dx < 20)
                X += dx;
            if (Y + dy >= 0 && Y + dy < 15)
                Y += dy;
        }

        // Реализация метода Move без параметров (например, для автоматического движения)
        public void Move()
        {
            // Логика для движения без параметров, если нужно
            // Например, просто двигаться вперед
            Y -= 1; // Двигаемся вверх по оси Y
        }
    }
}