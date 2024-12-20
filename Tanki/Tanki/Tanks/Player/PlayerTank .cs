using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki.Tanks;
using Tanki.Map;
using Tanki;

namespace Tanki.Tanks.Player
{
    public class PlayerTank : Tank
    {
        public PlayerTank(int x, int y, Direction direction) : base(x, y, direction) { }

        public override void Shoot(List<Bullet> bullets, int cooldown)
        {
            int bulletX = X;
            int bulletY = Y;

            switch (Direction)
            {
                case Direction.Up:
                    bulletY--;
                    break;
                case Direction.Down:
                    bulletY++;
                    break;
                case Direction.Left:
                    bulletX--;
                    break;
                case Direction.Right:
                    bulletX++;
                    break;
            }

            // Создаем новый снаряд с заданным cooldown
            bullets.Add(new Bullet(bulletX, bulletY, Direction, cooldown));
        }
    }
}
