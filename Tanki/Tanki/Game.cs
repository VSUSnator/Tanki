using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tanki.Map;
using static Tanki.Tank;

namespace Tanki
{
    public class Game
    {
        private GameState gameState;
        private Renderer renderer;
        private Stopwatch stopwatch;
        private MapLoader mapLoader; // Объявление поля

        private readonly Dictionary<ConsoleKey, Action> keyActions;

        public Game()
        {
            // Определяем путь к директории карт
            string mapDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Map");
            Console.WriteLine($"Путь к директории карт: {mapDirectory}");

            // Получение всех файлов карт из директории
            List<string> mapFiles = Directory.GetFiles(mapDirectory, "*.txt").ToList();

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
            StartGame();
            stopwatch.Start();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (keyActions.TryGetValue(key.Key, out var action))
                    {
                        action.Invoke();
                    }
                }

                UpdateGame();
                System.Threading.Thread.Sleep(10); // Задержка для уменьшения нагрузки на процессор
            }
        }

        private void UpdateGame()
        {
            if (gameState == null)
            {
                return;
            }

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
            gameState?.EndGame();
            Console.WriteLine("Игра окончена! Спасибо за игру.");
        }

        private void EndGameWithLoss()
        {
            gameState?.EndGame();
            Console.WriteLine("Ваш танк уничтожен! Игра окончена! Спасибо за игру.");
        }

        private void EndGameWithVictory()
        {
            gameState?.EndGame();
            Console.WriteLine("Все враги уничтожены! Игра окончена! Спасибо за игру.");
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