using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki;

namespace Tanki.Map
{
    public abstract class MapObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }

        protected MapObject(int x, int y, char symbol)
        {
            X = x;
            Y = y;
            Symbol = symbol;
        }

        public abstract void Draw();
    }

    public class Wall : MapObject
    {
        public Wall(int x, int y) : base(x, y, '#') { }

        public override void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(Symbol); // Отображение стены
        }
    }

    public class Obstacle : MapObject
    {
        public Obstacle(int x, int y) : base(x, y, 'X') { }

        public override void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(Symbol); // Отображение препятствия
        }
    }

    public class EmptySpace : MapObject
    {
        public EmptySpace(int x, int y) : base(x, y, '.') { }

        public override void Draw()
        {
            // Пустое пространство не требует отображения
        }
    }
    public class Water : MapObject
    {
        public Water(int x, int y) : base(x, y, 'W') // Символ для воды
        {
        }

        public override void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(Symbol); // Отображение воды
        }
    }
}


