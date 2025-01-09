using Tanki.Map;
using static Tanki.Tank;

namespace Tanki
{
    public class TankPlayer : Tank
    {
        private GameState gameState; // Ссылка на GameState

        public TankPlayer(int x, int y, GameMap map, GameState gameState) : base(x, y, map)
        {
            this.gameState = gameState; // Инициализируем GameState
            lives = 3; // Изначально 3 жизни
        }

        public void HandleInput()
        {
            // Если танк уничтожен, то игрок не может управлять им
            if (IsDestroyed) return;

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                int dx = 0, dy = 0;

                switch (key)
                {
                    case ConsoleKey.W: dy = -1; ChangeDirection(Direction.Up); break;
                    case ConsoleKey.S: dy = 1; ChangeDirection(Direction.Down); break;
                    case ConsoleKey.A: dx = -1; ChangeDirection(Direction.Left); break;
                    case ConsoleKey.D: dx = 1; ChangeDirection(Direction.Right); break;
                    case ConsoleKey.Spacebar: Shoot(); break; // Стрельба
                    case ConsoleKey.Escape: gameState.EndGame(); break; // Завершение игры
                }

                // Выполняем движение, если есть изменения
                Move(dx, dy);
            }
        }
    }
}
