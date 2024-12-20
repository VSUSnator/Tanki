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
    public abstract class Tank
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; }
        public Tank(int x, int y, Direction direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                case Direction.Up:
                    if (Y > 0) Y--;
                    break;
                case Direction.South:
                case Direction.Down:
                    if (Y < Game.MapHeight - 1) Y++;
                    break;
                case Direction.East:
                case Direction.Right:
                    if (X < Game.MapWidth - 1) X++;
                    break;
                case Direction.West:
                case Direction.Left:
                    if (X > 0) X--;
                    break;
            }
        }

        public abstract void Shoot(List<Bullet> bullets, int cooldown); // Абстрактный метод для стрельбы
    }
}
