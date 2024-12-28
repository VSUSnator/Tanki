using Tanki;
using Tanki.Map;
using static Tanki.Tank;

public class TankPlayer : Tank
{
    private GameState gameState; // Ссылка на GameState

    public TankPlayer(int x, int y, GameMap map, GameState gameState) : base(x, y, map)
    {
        this.gameState = gameState; // Инициализируем GameState
    }

    // Метод для обработки ввода игрока
    public void HandleInput()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.W: // Движение вверх
                    gameState.TryMove(0, -1);
                    ChangeDirection(Direction.Up);
                    break;
                case ConsoleKey.S: // Движение вниз
                    gameState.TryMove(0, 1);
                    ChangeDirection(Direction.Down);
                    break;
                case ConsoleKey.A: // Движение влево
                    gameState.TryMove(-1, 0);
                    ChangeDirection(Direction.Left);
                    break;
                case ConsoleKey.D: // Движение вправо
                    gameState.TryMove(1, 0);
                    ChangeDirection(Direction.Right);
                    break;
                case ConsoleKey.Spacebar: // Стрельба
                    gameState.TryShoot();
                    break;
                case ConsoleKey.Escape: // Завершение игры
                    gameState.EndGame(); // Добавьте логику завершения игры, если это необходимо
                    break;
            }
        }
    }
}