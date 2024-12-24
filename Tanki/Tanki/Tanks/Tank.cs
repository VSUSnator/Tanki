using Tanki.Map;

namespace Tanki
{
    public class Tank : IMovable
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public string Direction { get; private set; } // Хранит текущее направление танка

        public Tank(int x, int y)
        {
            X = x;
            Y = y;
            Direction = "Up"; // Начальное направление
        }

        // Реализация метода Move
        public void Move(int dx, int dy)
        {
            // Проверка границ экрана
            if (X + dx >= 0 && X + dx < 20)
                X += dx;
            if (Y + dy >= 0 && Y + dy < 15)
                Y += dy;

            // Обновление направления в зависимости от перемещения
            if (dy < 0) Direction = "Up";
            else if (dy > 0) Direction = "Down";
            else if (dx < 0) Direction = "Left";
            else if (dx > 0) Direction = "Right";
        }

        public Bullet Shoot()
        {
            // Создание снаряда в зависимости от направления
            Bullet bullet = new Bullet(X, Y);
            bullet.Direction = Direction; // Устанавливаем направление снаряда
            return bullet;
        }
    }
}