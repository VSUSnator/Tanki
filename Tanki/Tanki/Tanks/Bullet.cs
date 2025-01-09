using System;
using Tanki.Map;
using static Tanki.Tank;

namespace Tanki
{
    public class Bullet : MapObject // Наследование от MapObject
    {
        public Direction Direction { get; set; } // Используем перечисление Direction
        public new char Symbol { get; private set; } // Символ для отображения снаряда
        public bool IsActive { get; private set; } // Состояние снаряда (активен или нет)

        public Bullet(int x, int y) : base(x, y, '*') // Символ снаряда '*'
        {
            Symbol = '*'; // Установка символа по умолчанию
            IsActive = true; // Снаряд активен при создании
        }

        public void Move()
        {
            // Двигаем снаряд в зависимости от его направления
            switch (Direction)
            {
                case Direction.Up:
                    Y -= 1;
                    break;
                case Direction.Down:
                    Y += 1;
                    break;
                case Direction.Left:
                    X -= 1;
                    break;
                case Direction.Right:
                    X += 1;
                    break;
            }
        }

        public bool IsInBounds(int width, int height)
        {
            return X >= 0 && X < width && Y >= 0 && Y < height; // Проверяем, находится ли снаряд в пределах игрового поля
        }

        public void Deactivate()
        {
            IsActive = false; // Деактивируем снаряд
        }

        public override void Draw()
        {
            if (IsActive) // Отображаем только активные снаряды
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(Symbol); // Отображение снаряда
            }
        }
    }
}