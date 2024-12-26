using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki.Map
{
    public class MapSize
    {
        public int Width { get; }
        public int Height { get; }

        public MapSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}