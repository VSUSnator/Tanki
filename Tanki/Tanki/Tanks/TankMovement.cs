using Tanki.Map;

namespace Tanki.Tanks
{
    public class TankMovement
    {
        private readonly Tank tank;
        private readonly GameMap gameMap;

        public TankMovement(Tank tank, GameMap gameMap)
        {
            this.tank = tank;
            this.gameMap = gameMap;
        }

        public void Move(Direction direction)
        {
            int newX = tank.X;
            int newY = tank.Y;

            switch (direction)
            {
                case Direction.North:
                case Direction.Up:
                    newY--; // Уменьшаем Y для движения вверх
                    break;
                case Direction.South:
                case Direction.Down:
                    newY++; // Увеличиваем Y для движения вниз
                    break;
                case Direction.East:
                case Direction.Right:
                    newX++; // Увеличиваем X для движения вправо
                    break;
                case Direction.West:
                case Direction.Left:
                    newX--; // Уменьшаем X для движения влево
                    break;
            }

            // Проверяем возможность движения перед изменением позиции
            if (gameMap.CanMoveTo(newX, newY))
            {
                tank.X = newX;
                tank.Y = newY;
            }
        }
    }
}