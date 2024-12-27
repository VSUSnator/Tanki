using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class Node
    {
        public int X { get; }
        public int Y { get; }
        public int GCost { get; set; } // Расстояние от стартовой точки
        public int HCost { get; set; } // Оценка расстояния до цели
        public Node Parent { get; set; } // Для восстановления пути

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int FCost => GCost + HCost; // Общая стоимость
    }
}
