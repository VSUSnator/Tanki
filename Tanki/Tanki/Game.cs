using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tanki.Map;
using static Tanki.Tank;

namespace Tanki
{
    public class Game
    {
        private GameState gameState;
        private Renderer renderer;
        private CancellationTokenSource cancellationTokenSource;
        private Stopwatch stopwatch;

        private readonly Dictionary<ConsoleKey, Action> keyActions;

        public Game()
        {
            string mapFilePath = "C:\\Новая папка\\Tanki-Main\\Tanki\\Tanki\\Tanki\\Map\\map.txt";
            gameState = new GameState(mapFilePath);
            renderer = new Renderer(new ConsoleColor[] { ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue });

            stopwatch = new Stopwatch();

            keyActions = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.W, () => MovePlayerTank(Direction.Up) },
                { ConsoleKey.S, () => MovePlayerTank(Direction.Down) },
                { ConsoleKey.A, () => MovePlayerTank(Direction.Left) },
                { ConsoleKey.D, () => MovePlayerTank(Direction.Right) },
                { ConsoleKey.Spacebar, () => Shoot() },
                { ConsoleKey.Escape, EndGame }
            };
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartGame();

            Task inputTask = Task.Run(() => ProcessInputAsync(cancellationTokenSource.Token));
            stopwatch.Start();

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                UpdateGame();
                Thread.Sleep(10);
            }

            EndGame();
            inputTask.Wait();
        }

        private void UpdateGame()
        {
            TimeSpan elapsedTime = stopwatch.Elapsed;
            stopwatch.Restart();

            gameState.Update(elapsedTime);

            // Проверяем, уничтожен ли танк игрока
            if (gameState.PlayerTank.IsDestroyed)
            {
                EndGameWithLoss();
                return; // Выход из метода, чтобы не продолжать обновления
            }

            // Проверяем, остались ли враги
            if (gameState.AreAllEnemiesDefeated())
            {
                EndGameWithVictory();
                return; // Выход из метода, чтобы не продолжать обновления
            }

            renderer.Draw(gameState);
        }


        private void StartGame()
        {
            gameState.StartGame();
            Console.WriteLine("Игра началась! Нажмите ESC для выхода.");
        }

        private void EndGame()
        {
            cancellationTokenSource.Cancel();
            gameState.EndGame();
            Console.WriteLine("Игра окончена! Спасибо за игру.");
        }

        private void EndGameWithLoss()
        {
            cancellationTokenSource.Cancel();
            gameState.EndGame();
            Console.WriteLine("Ваш танк уничтожен! Игра окончена! Спасибо за игру.");
        }

        private void EndGameWithVictory()
        {
            cancellationTokenSource.Cancel();
            gameState.EndGame();
            Console.WriteLine("Все враги уничтожены! Игра окончена! Спасибо за игру.");
        }

        private async Task ProcessInputAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (keyActions.TryGetValue(key.Key, out var action))
                        {
                            action.Invoke();
                        }
                    }
                    await Task.Delay(10, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Задача была отменена
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка ввода: {ex.Message}");
            }
        }

        private void MovePlayerTank(Direction direction)
        {
            gameState.PlayerTank.ChangeDirection(direction);

            switch (direction)
            {
                case Direction.Up:
                    gameState.PlayerTank.Move(0, -1);
                    break;
                case Direction.Down:
                    gameState.PlayerTank.Move(0, 1);
                    break;
                case Direction.Left:
                    gameState.PlayerTank.Move(-1, 0);
                    break;
                case Direction.Right:
                    gameState.PlayerTank.Move(1, 0);
                    break;
            }
        }

        private void Shoot()
        {
            Bullet bullet = gameState.PlayerTank.Shoot();
            if (bullet != null)
            {
                gameState.Bullets.Add(bullet);
            }
        }
    }
}