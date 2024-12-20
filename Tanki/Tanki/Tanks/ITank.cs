using System.Collections.Generic;

namespace Tanki.Tanks
{
    public interface ITank
    {
        int X { get; set; }
        int Y { get; set; }
        Direction Direction { get; set; }

        void Move(Direction direction);
        void Shoot(List<Bullet> bullets, int cooldown);
    }
}