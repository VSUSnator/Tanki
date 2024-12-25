using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tanki.Map;

namespace Tanki
{
    public class Game
    {
        private GameState gameState;
        private Renderer renderer;
        private CancellationTokenSource cancellationTokenSource;
        private ConcurrentQueue<ConsoleKey> inputQueue;
        private DateTime lastInputTime;
        private TimeSpan inputTimeout;

        public Game()
        {
            string mapFilePath = "C:\\Новая папка\\Tanki-Main\\Tanki\\Tanki\\Tanki\\Map\\map.txt";
            gameState = new GameState(mapFilePath);

            ConsoleColor[] colors = { ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue };
            renderer = new Renderer(colors);

            inputQueue = new ConcurrentQueue<ConsoleKey>();
            lastInputTime = DateTime.Now;
            inputTimeout = TimeSpan.FromMilliseconds(500);
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartGame();
            Task.Run(() => ProcessInputAsync(cancellationTokenSource.Token));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                UpdateGame();
                CleanInputQueue();
                Thread.Sleep(150); // FPS
            }

            EndGame();
        }

        private void UpdateGame()
        {
            gameState.Update();
            renderer.Draw(gameState);
            ExecuteCommands();
        }

        private void CleanInputQueue()
        {
            if (DateTime.Now - lastInputTime > inputTimeout)
            {
                // Игнорируем старые команды
            }
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

        private async Task ProcessInputAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        inputQueue.Enqueue(key.Key);
                        lastInputTime = DateTime.Now;
                    }
                    await Task.Delay(50, cancellationToken);
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

        private void ExecuteCommands()
        {
            while (inputQueue.TryDequeue(out ConsoleKey key))
            {
                HandleKeyPress(key);
            }
        }

        private void HandleKeyPress(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    gameState.PlayerTank.Move(0, -1); // Теперь только два аргумента
                    break;
                case ConsoleKey.S:
                    gameState.PlayerTank.Move(0, 1);
                    break;
                case ConsoleKey.A:
                    gameState.PlayerTank.Move(-1, 0);
                    break;
                case ConsoleKey.D:
                    gameState.PlayerTank.Move(1, 0);
                    break;
                case ConsoleKey.Spacebar:
                    gameState.AddBullet(); // Добавление снаряда
                    break;
                case ConsoleKey.Escape:
                    EndGame(); // Завершение игры
                    break;
            }
        }
    }
}
