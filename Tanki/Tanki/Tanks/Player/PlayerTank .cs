using Tanki;
using Tanki.Map;
using static Tanki.Tank;

public class TankPlayer : Tank
{
    private ActionDelay moveDelay;
    private ActionDelay shootDelay;
    private GameState gameState; // Ссылка на GameState

    public TankPlayer(int x, int y, GameMap map, GameState gameState) : base(x, y, map)
    {
        this.gameState = gameState; // Инициализируем GameState
        moveDelay = new ActionDelay(TimeSpan.FromMilliseconds(300)); // Задержка для перемещения
        shootDelay = new ActionDelay(TimeSpan.FromMilliseconds(900)); // Задержка для стрельбы
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
                    // Логика завершения игры, если это необходимо
                    break;
            }
        }
    }
}