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
    public class TankMovement
    {
        private Tank tank;
        private int mapWidth;
        private int mapHeight;

        public TankMovement(Tank tank, int mapWidth, int mapHeight)
        {
            this.tank = tank;
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    if (tank.Y > 0) tank.Y--;
                    break;
                case Direction.South:
                    if (tank.Y < mapHeight - 1) tank.Y++;
                    break;
                case Direction.East:
                    if (tank.X < mapWidth - 1) tank.X++;
                    break;
                case Direction.West:
                    if (tank.X > 0) tank.X--;
                    break;
            }
        }
    }
}


