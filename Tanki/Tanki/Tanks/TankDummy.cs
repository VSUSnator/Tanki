using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tanki.Map;

namespace Tanki
{
    public class TankDummy
    {
        public int X { get; }
        public int Y { get; }

        public TankDummy(int x, int y, GameMap map)
        {
            X = x;
            Y = y;
        }
    }
}
