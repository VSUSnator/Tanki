using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki.Tanks;
using Tanki.Map;
using Tanki;

namespace Tanki.Tanks
{
    public class Bullet
    {
        public int BulletX { get; set; }
        public int BulletY { get; set; }
        public Direction Direction { get; set; } // Используем Direction из перечисления

        // Конструктор для инициализации снаряда
        public Bullet(int x, int y, Direction direction)
        {
            BulletX = x;
            BulletY = y;
            Direction = direction;
        }

        public void UpdateBullet()
        {
            switch (Direction)
            {
                case Direction.North:
                    BulletY--; // Двигаем снаряд вверх
                    break;
                case Direction.South:
                    BulletY++; // Двигаем снаряд вниз
                    break;
                case Direction.East:
                    BulletX++; // Двигаем снаряд вправо
                    break;
                case Direction.West:
                    BulletX--; // Двигаем снаряд влево
                    break;
            }
        }
    }
}
