//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Tanki
//{

//    public class Node
//    {
//        public int X { get; }
//        public int Y { get; }
//        public Node Parent { get; set; } // Для отслеживания пути

//        public Node(int x, int y)
//        {
//            X = x;
//            Y = y;
//            Parent = null;
//        }

//        public override bool Equals(object obj)
//        {
//            if (obj is Node other)
//            {
//                return X == other.X && Y == other.Y;
//            }
//            return false;
//        }

//        public override int GetHashCode()
//        {
//            return (X, Y).GetHashCode();
//        }
//    }
//}
