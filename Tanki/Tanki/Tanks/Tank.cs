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
    public class Tank
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; } // Текущее направление танка

        public Tank()
        {
            Direction = Direction.North; // По умолчанию направлен на север
        }

        public void Shoot(List<Bullet> bullets)
        {
            Bullet bullet = new Bullet(this.X, this.Y, this.Direction); // Используем Direction
            bullets.Add(bullet); // Добавляем снаряд в список
        }
    }
}
