using System;
using System.Collections.Concurrent;
using System.Threading;
using Tanki.Map;

namespace Tanki
{
    public class Game
    {
        private GameState gameState;
        private Renderer renderer;
        private bool isGameRunning;
        private ConcurrentQueue<ConsoleKey> inputQueue;
        private DateTime lastInputTime; // Время последнего ввода
        private TimeSpan inputTimeout; // Таймаут для ввода

        public Game()
        {
            string mapFilePath = "C:\\Новая папка\\Tanki-Main\\Tanki\\Tanki\\Tanki\\Map\\map.txt";
            gameState = new GameState(mapFilePath); // Передаем путь к карте
            renderer = new Renderer(gameState);
            isGameRunning = false; // Игра не запущена
            inputQueue = new ConcurrentQueue<ConsoleKey>();
            lastInputTime = DateTime.Now; // Инициализируем время последнего ввода
            inputTimeout = TimeSpan.FromMilliseconds(500); // Устанавливаем таймаут на 500 мс
        }

        public void Start()
        {
            StartGame(); // Начинаем игру
            Task.Run(() => ProcessInputAsync()); // Запускаем асинхронный ввод

            while (isGameRunning)
            {
                renderer.Draw();
                ExecuteCommands(); // Выполняем команды из очереди

                // Проверяем, прошло ли время без ввода, если да - очищаем очередь
                if (DateTime.Now - lastInputTime > inputTimeout)
                {
                    inputQueue = new ConcurrentQueue<ConsoleKey>(); // Очищаем очередь
                }

                gameState.Update(); // Обновляем состояние игры
                Thread.Sleep(100);
            }

            EndGame(); // Завершаем игру
        }

        public void GameLoop()
        {
            while (gameState.IsGameActive)
            {
                gameState.Update(); // Обновляем состояние игры
                renderer.Draw(); // Отрисовываем всё
                System.Threading.Thread.Sleep(100); // Задержка для управления частотой кадров
            }
        }

        private void StartGame()
        {
            isGameRunning = true;
            gameState.StartGame(); // Логика начала игры
            Console.WriteLine("Игра началась! Нажмите ESC для выхода.");
        }

        private void EndGame()
        {
            isGameRunning = false;
            gameState.EndGame(); // Логика завершения игры
            Console.WriteLine("Игра окончена! Спасибо за игру.");
        }

        private async Task ProcessInputAsync()
        {
            while (isGameRunning)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    inputQueue.Enqueue(key.Key); // Добавляем нажатую клавишу в очередь
                    lastInputTime = DateTime.Now; // Обновляем время последнего ввода
                }
                await Task.Delay(50); // Небольшая задержка, чтобы не перегружать процессор
            }
        }

        private void ExecuteCommands()
        {
            while (inputQueue.TryDequeue(out ConsoleKey key)) // Извлекаем команды из очереди
            {
                switch (key)
                {
                    case ConsoleKey.W: // Вверх
                        gameState.Tank.Move(0, -1);
                        break;
                    case ConsoleKey.S: // Вниз
                        gameState.Tank.Move(0, 1);
                        break;
                    case ConsoleKey.A: // Влево
                        gameState.Tank.Move(-1, 0);
                        break;
                    case ConsoleKey.D: // Вправо
                        gameState.Tank.Move(1, 0);
                        break;
                    case ConsoleKey.Spacebar: // Стрельба
                        gameState.AddBullet();
                        break;
                    case ConsoleKey.Escape: // Выход
                        EndGame(); // Завершение игры
                        break;
                }
            }
        }
    }
}
