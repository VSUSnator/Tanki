using Tanki.Map;

namespace Tanki
{
    public class Tank : IMovable
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public string Direction { get; private set; } // Хранит текущее направление танка
        private GameMap gameMap; // Ссылка на игровую карту

        public Tank(int x, int y, GameMap map)
        {
            X = x;
            Y = y;
            Direction = "Up"; // Начальное направление
            gameMap = map; // Инициализация карты
        }

        // Реализация метода Move
        public void Move(int dx, int dy)
        {
            // Проверка на столкновение с границами карты или объектами
            if (CanMove(dx, dy))
            {
                X += dx;
                Y += dy;

                // Обновление направления в зависимости от перемещения
                if (dy < 0) Direction = "Up";
                else if (dy > 0) Direction = "Down";
                else if (dx < 0) Direction = "Left";
                else if (dx > 0) Direction = "Right";
            }
        }

        private bool CanMove(int dx, int dy)
        {
            // Проверка границ экрана
            int newX = X + dx;
            int newY = Y + dy;

            // Проверка выхода за пределы карты
            if (newX < 0 || newX >= gameMap.Width || newY < 0 || newY >= gameMap.Height)
            {
                return false; // Выход за пределы карты
            }

            // Получение символа из карты для проверки коллизии
            char mapSymbol = gameMap.GetMapSymbol(newX, newY);
            if (mapSymbol == '#' || mapSymbol == 'X') // '#' - стена, 'X' - препятствие
            {
                return false; // Столкновение со стеной или препятствием
            }

            return true; // Движение допустимо
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