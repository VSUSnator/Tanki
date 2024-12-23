using System.Collections.Generic;

namespace Tanki.Tanks
{
    public abstract class BaseTank<T> : Tank where T : GameState
    {
        protected BaseTank(int x, int y, Direction direction, T gameState)
            : base(x, y, direction, gameState) // Вызов конструктора базового класса Tank
        {
            // Здесь можно добавить дополнительную инициализацию, если это необходимо
        }
    }
}