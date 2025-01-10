using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private MapLoader mapLoader; // Объявление поля

        private readonly Dictionary<ConsoleKey, Action> keyActions;

        public Game()
        {
            string mapDirectory = @"C:\Новая папка\Tanki-Main\Tanki\Tanki\Tanki\Map";
            Console.WriteLine($"Путь к директории карт: {mapDirectory}");

            if (!Directory.Exists(mapDirectory))
            {
                Console.WriteLine($"Директория с картами не найдена: {mapDirectory}");
                EndGame(); // Завершение игры, если директория не существует
                return; // Выход из конструктора
            }

            // Получение всех файлов карт из директории
            List<string> mapFiles = new List<string>(Directory.GetFiles(mapDirectory, "*.txt"));
            Console.WriteLine($"Найдено файлов карт: {mapFiles.Count}");

            // Проверка, найдены ли карты
            if (mapFiles.Count == 0)
            {
                Console.WriteLine("Нет доступных карт для игры.");
                EndGame(); // Завершение игры, если карты не найдены
                return; // Выход из конструктора
            }

            mapLoader = new MapLoader(mapFiles);
            LoadNewMap(); // Загрузка первой карты
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

        private void LoadNewMap()
        {
            try
            {
                string mapFilePath = mapLoader.GetNextMap();

                if (mapFilePath != null)
                {
                    if (File.Exists(mapFilePath)) // Проверка на существование файла
                    {
                        gameState = new GameState(mapFilePath);
                        StartGame(); // Перезапустить игру с новой картой
                    }
                    else
                    {
                        Console.WriteLine($"Файл карты не найден: {mapFilePath}");
                        EndGame(); // Завершение игры при отсутствии файла
                    }
                }
                else
                {
                    EndGameWithVictory(); // Если карт больше нет, заканчиваем игру с победой
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке карты: {ex.Message}");
                EndGame(); // Завершение игры в случае ошибки
            }
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
                LoadNewMap(); // Загружаем новую карту
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
            if (gameState != null)
            {
                gameState.EndGame();
            }
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


//public Game()
//{
//    // Получаем базовый путь к директории, в которой находится исполняемый файл
//    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
//    // Формируем путь к директории с картами
//    string mapDirectory = Path.Combine(baseDirectory, "Map");

//    Console.WriteLine($"Путь к директории карт: {mapDirectory}");

//    if (!Directory.Exists(mapDirectory))
//    {
//        Console.WriteLine($"Директория с картами не найдена: {mapDirectory}");
//        EndGame(); // Завершение игры, если директория не существует
//        return; // Выход из конструктора
//    }

//    // Получение всех файлов карт из директории
//    List<string> mapFiles = new List<string>(Directory.GetFiles(mapDirectory, "*.txt"));
//    Console.WriteLine($"Найдено файлов карт: {mapFiles.Count}");

//    // Проверка, найдены ли карты
//    if (mapFiles.Count == 0)
//    {
//        Console.WriteLine("Нет доступных карт для игры.");
//        EndGame(); // Завершение игры, если карты не найдены
//        return; // Выход из конструктора
//    }

//    mapLoader = new MapLoader(mapFiles);
//    LoadNewMap(); // Загрузка первой карты

// я хотел выполнить Уровень 2: Помести карту в файл и грузи карты для уровней из файловой системы с помощью System.IO.
// Но либо из за того что у меня 3 папки с одинаковым именнем либо из за чего то другого у меня код не мог создать правильную  директории с картами
// Поэтому сделаю примитивный абсолютный путь