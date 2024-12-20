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
        public Direction Direction { get; set; }
        private int updateCooldown; // Количество кадров между обновлениями
        private int updateTimer; // Таймер обновления

        public Bullet(int x, int y, Direction direction, int cooldown = 5) // Добавляем параметр cooldown
        {
            BulletX = x;
            BulletY = y;
            Direction = direction;
            updateCooldown = cooldown; // Устанавливаем значение cooldown
            updateTimer = 0; // Инициализируем таймер
        }

        public void UpdateBullet()
        {
            updateTimer++;

            if (updateTimer >= updateCooldown) // Проверяем, нужно ли обновить позицию
            {
                switch (Direction)
                {
                    case Direction.North:
                    case Direction.Up:
                        BulletY--; // Двигаем снаряд вверх
                        break;
                    case Direction.South:
                    case Direction.Down:
                        BulletY++; // Двигаем снаряд вниз
                        break;
                    case Direction.East:
                    case Direction.Right:
                        BulletX++; // Двигаем снаряд вправо
                        break;
                    case Direction.West:
                    case Direction.Left:
                        BulletX--; // Двигаем снаряд влево
                        break;
                }
                updateTimer = 0; // Сбрасываем таймер
            }
        }
    }
}