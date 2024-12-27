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
        private DateTime lastInputTime;
        private TimeSpan inputTimeout;

        public Game()
        {
            string mapFilePath = "C:\\Новая папка\\Tanki-Main\\Tanki\\Tanki\\Tanki\\Map\\map.txt";
            gameState = new GameState(mapFilePath);

            ConsoleColor[] colors = { ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue };
            renderer = new Renderer(colors);

            lastInputTime = DateTime.Now;
            inputTimeout = TimeSpan.FromMilliseconds(10); // Меньшая задержка для ввода
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartGame();
            Task inputTask = Task.Run(() => ProcessInputAsync(cancellationTokenSource.Token));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                UpdateGame();
                Thread.Sleep(10); // Увеличенная частота обновления
            }

            EndGame();
            // Ждем завершения задачи обработки ввода
            inputTask.Wait();
        }

        private void UpdateGame()
        {
            gameState.Update(); // Обновляем состояние игры
            renderer.Draw(gameState); // Отрисовываем всё на экране
        }

        private void StartGame()
        {
            gameState.StartGame();
            Console.WriteLine("Игра началась! Нажмите ESC для выхода.");

            // Инициализация врагов
            InitializeEnemies();
        }

        private void InitializeEnemies()
        {
            // Пример: добавляем 3 врага на карту
            for (int i = 0; i < 3; i++)
            {
                int x = i * 2; // Простая логика для размещения врагов
                int y = i * 2; // Вы можете изменить это на более сложную логику
                gameState.Enemies.Add(new TankEnemy(x, y, gameState.GameMap));
            }
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
                        HandleKeyPress(key.Key); // Обработка ввода сразу
                        lastInputTime = DateTime.Now;
                    }
                    await Task.Delay(10, cancellationToken); // Уменьшенная задержка для более быстрого ввода
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

        private void HandleKeyPress(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    gameState.PlayerTank.ChangeDirection(Direction.Up);
                    gameState.PlayerTank.Move(0, -1); // Движение вверх
                    break;
                case ConsoleKey.S:
                    gameState.PlayerTank.ChangeDirection(Direction.Down);
                    gameState.PlayerTank.Move(0, 1); // Движение вниз
                    break;
                case ConsoleKey.A:
                    gameState.PlayerTank.ChangeDirection(Direction.Left);
                    gameState.PlayerTank.Move(-1, 0); // Движение влево
                    break;
                case ConsoleKey.D:
                    gameState.PlayerTank.ChangeDirection(Direction.Right);
                    gameState.PlayerTank.Move(1, 0); // Движение вправо
                    break;
                case ConsoleKey.Spacebar:
                    Bullet bullet = gameState.PlayerTank.Shoot(); // Стрельба
                    gameState.Bullets.Add(bullet); // Добавление снаряда в список
                    break;
                case ConsoleKey.Escape:
                    EndGame(); // Завершение игры
                    break;
            }
        }
    }
}