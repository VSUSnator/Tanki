using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki.Tanks
{
    public class Bullet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; }

        public void Update()
        {
            // Логика движения снаряда
        }
    }
}
