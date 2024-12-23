using System;
using System.Collections.Generic;
using Tanki.Map;
using Tanki.Tanks;
using Tanki.Tanks.Enemy;
using Tanki.Tanks.Player;

namespace Tanki
{
    public class Game
    {
        private GameState gameState; // Экземпляр GameState
        public static int MapWidth { get; private set; } = 50;
        public static int MapHeight { get; private set; } = 24;
        private readonly GameMap gameMap;
        private readonly PlayerTank mainPlayerTank;
        private readonly Renderer renderer;
        private InputHandler inputHandler;
        private int frameCount = 0;
        private const int FrameRenderRate = 4;
        private const int FrameDelay = 100;

        public Game()
        {
            gameMap = new GameMap(MapWidth, MapHeight);
            gameState = new GameState(gameMap); // Инициализация GameState с GameMap
            mainPlayerTank = new PlayerTank(MapWidth / 8, MapHeight / 7, Direction.Up, gameState); // Передаем gameState

            var enemyTanks = CreateEnemyTanks(); // Создание врагов
            gameState.AddEnemyTanks(enemyTanks); // Добавляем врагов в GameState

            renderer = new Renderer(MapWidth, MapHeight);
            inputHandler = new InputHandler(mainPlayerTank, new TankMovement(mainPlayerTank, gameMap)); // Передаем оба аргумента
            inputHandler.OnShoot += HandleShoot; // Подписываемся на событие стрельбы
        }

        private List<EnemyTank> CreateEnemyTanks()
        {
            return new List<EnemyTank>
            {
                new EnemyTank(MapWidth / 10, MapHeight / 4, Direction.Right, gameState), // Передаем gameState
                new EnemyTank(MapWidth / 3, MapHeight / 3, Direction.Left, gameState) // Передаем gameState
            };
        }

        private void Shoot(List<Bullet> bullets)
        {
            var bullet = new Bullet(mainPlayerTank.X, mainPlayerTank.Y, mainPlayerTank.Direction, gameState);
            bullets.Add(bullet); // Добавляем пулю в список
        }

        public void Start()
        {
            DateTime lastFrameTime = DateTime.Now;

            while (true)
            {
                UpdateGameState();

                if (frameCount % FrameRenderRate == 0)
                {
                    RenderGame();
                }

                frameCount++;
                MaintainFrameRate(lastFrameTime);
                lastFrameTime = DateTime.Now;
            }
        }

        private void UpdateGameState()
        {
            List<Bullet> bulletsList = new List<Bullet>(gameState.Bullets); // Создание нового списка
            inputHandler.ProcessInput(bulletsList, frameCount); // Передаем новый список в метод
            gameState.Update(); // Обновление состояния игры
        }

        private void RenderGame()
        {
            Console.SetCursorPosition(0, 0);
            gameMap.Render();
            List<Bullet> bulletsList = new List<Bullet>(gameState.Bullets); // Создание нового списка
            renderer.Render(mainPlayerTank, bulletsList); // Передаем новый список
            foreach (var enemy in gameState.EnemyTanks)
            {
                renderer.RenderEnemyTank(enemy);
            }
        }

        private void MaintainFrameRate(DateTime lastFrameTime)
        {
            TimeSpan elapsedTime = DateTime.Now - lastFrameTime;

            if (elapsedTime.TotalMilliseconds < FrameDelay)
            {
                System.Threading.Thread.Sleep(FrameDelay - (int)elapsedTime.TotalMilliseconds);
            }
        }

        private void HandleShoot(List<Bullet> bullets, int cooldown)
        {
            Shoot(bullets); // Вызов метода Shoot
        }
    }
}