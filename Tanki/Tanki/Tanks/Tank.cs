using System;
using System.Collections.Generic;
using Tanki.Map;

namespace Tanki.Tanks
{
    public abstract class Tank : ITank
    {
        protected TankState state;

        public int X
        {
            get => state.X;
            set
            {
                if (value >= 0 && value < Game.MapWidth) state.X = value;
            }
        }

        public int Y
        {
            get => state.Y;
            set
            {
                if (value >= 0 && value < Game.MapHeight) state.Y = value;
            }
        }

        public Direction Direction
        {
            get => state.Direction;
            set => state.Direction = value;
        }

        protected Tank(int x, int y, Direction direction)
        {
            state = new TankState(x, y, direction);
        }

        public void Move(Direction direction)
        {
            if (!state.IsAlive) return; // Если танк мёртв, не двигаться

            switch (direction)
            {
                case Direction.North:
                case Direction.Up:
                    if (Y > 0) Y--;
                    break;
                case Direction.South:
                case Direction.Down:
                    if (Y < Game.MapHeight - 1) Y++;
                    break;
                case Direction.East:
                case Direction.Right:
                    if (X < Game.MapWidth - 1) X++;
                    break;
                case Direction.West:
                case Direction.Left:
                    if (X > 0) X--;
                    break;
            }

            Direction = direction; // Обновляем направление после движения
        }

        public event Action<List<Bullet>, int> OnShoot;

        public virtual void Shoot(List<Bullet> bullets, int cooldown)
        {
            if (!state.IsAlive) return; // Если танк мёртв, не стрелять

            OnShoot?.Invoke(bullets, cooldown);
        }

        protected void Die()
        {
            state.IsAlive = false; // Устанавливаем флаг мёртвого танка
            Console.WriteLine($"Tank at ({X}, {Y}) has been destroyed.");
        }
    }
}
