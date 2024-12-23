﻿using System;
using Tanki.Map;

namespace Tanki
{
    public class Bullet : MapObject // Наследование от MapObject
    {
        public string Direction { get; set; } // Хранит направление снаряда

        public Bullet(int x, int y) : base(x, y, '*') // Символ снаряда '*'
        {
            X = x;
            Y = y;
        }

        public void Move()
        {
            // Двигаем снаряд в зависимости от его направления
            switch (Direction)
            {
                case "Up":
                    Y -= 1;
                    break;
                case "Down":
                    Y += 1;
                    break;
                case "Left":
                    X -= 1;
                    break;
                case "Right":
                    X += 1;
                    break;
            }
        }

        public override void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(Symbol); // Отображение снаряда
        }
    }
}