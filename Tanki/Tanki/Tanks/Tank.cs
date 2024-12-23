using System;
using System.Collections.Generic;
using Tanki.Map;

namespace Tanki.Tanks
{
    public abstract class Tank : ITank
    {
        protected TankState state;
        protected GameState gameState; // Состояние игры

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

        public bool IsAlive => state.IsAlive; // Реализация свойства IsAlive

        protected Tank(int x, int y, Direction direction, GameState gameState) // Конструктор с инициализацией GameState
        {
            state = new TankState(x, y, direction);
            this.gameState = gameState; // Инициализация состояния игры
        }

        public virtual void Move(Direction direction)
        {
            if (!state.IsAlive) return; // Если танк мёртв, не двигаться

            if (CanMove(direction))
            {
                (int newX, int newY) = GetNewCoordinates(direction);
                X = newX; // Устанавливаем новые координаты
                Y = newY; // Устанавливаем новые координаты
                Direction = direction; // Обновляем направление после движения
            }
        }

        private (int newX, int newY) GetNewCoordinates(Direction direction)
        {
            int newX = X;
            int newY = Y;

            switch (direction)
            {
                case Direction.North:
                case Direction.Up:
                    newY--;
                    break;
                case Direction.South:
                case Direction.Down:
                    newY++;
                    break;
                case Direction.East:
                case Direction.Right:
                    newX++;
                    break;
                case Direction.West:
                case Direction.Left:
                    newX--;
                    break;
            }

            return (newX, newY);
        }

        private bool CanMove(Direction direction)
        {
            var (newX, newY) = GetNewCoordinates(direction);

            // Проверяем, находятся ли новые координаты в пределах карты и можно ли туда двигаться
            return IsInBounds(newX, newY) && gameState.Map.CanMoveTo(newX, newY); // Используем gameState
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Game.MapWidth && y >= 0 && y < Game.MapHeight;
        }

        public event Action<List<Bullet>, int> OnShoot; // Событие стрельбы

        public virtual void Shoot(List<Bullet> bullets, int cooldown)
        {
            if (!state.IsAlive) return; // Если танк мёртв, не стрелять

            OnShoot?.Invoke(bullets, cooldown); // Вызов события
            bullets.Add(new Bullet(X, Y, Direction, gameState)); // Передаем gameState в Bullet
        }

        protected void Die()
        {
            state.IsAlive = false; // Устанавливаем флаг мёртвого танка
            Console.WriteLine($"Tank at ({X}, {Y}) has been destroyed.");
        }
    }
}