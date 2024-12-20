using System;
using System.Collections.Generic;
using Tanki.Map;

namespace Tanki.Tanks
{
    public class TankState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; }
        public bool IsAlive { get; set; }

        public TankState(int x, int y, Direction direction)
        {
            X = x;
            Y = y;
            Direction = direction;
            IsAlive = true; // танк жив при создании
        }
    }
}