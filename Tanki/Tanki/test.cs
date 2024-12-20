//using System;
//using System.Collections.Generic;
//using Tanki.Tanks;
//using Tanki.Map;
//using Tanki.Tanks.Enemy;
//using Tanki.Tanks.Player;
//using static System.Collections.Specialized.BitVector32;

//namespace Tanki
//{
//    public class Game
//    {
//        private PlayerTank mainPlayerTank;
//        private List<EnemyTank> enemyTanks;
//        private Renderer renderer;
//        private List<Bullet> bullets = new List<Bullet>();
//        public static int MapWidth { get; private set; } = 50;
//        public static int MapHeight { get; private set; } = 24;
//        private GameMap gameMap;
//        private TankMovement tankMovement; // Объявляем переменную tankMovement
//        private int frameCount = 0;

//        public Game()
//        {
//            gameMap = new GameMap(MapWidth, MapHeight); // Инициализация карты

//            mainPlayerTank = new PlayerTank(MapWidth / 2, MapHeight / 2, Direction.Up);
//            enemyTanks = new List<EnemyTank>
//            {
//                new EnemyTank(MapWidth / 4, MapHeight / 4, Direction.Right, gameMap), // Передаем gameMap
//                new EnemyTank(MapWidth / 3, MapHeight / 3, Direction.Left, gameMap) // Передаем gameMap
//            };
//            renderer = new Renderer(MapWidth, MapHeight);
//        }

//        public void Start()
//        {
//            tankMovement = new TankMovement(mainPlayerTank, gameMap);
//            DateTime lastFrameTime = DateTime.Now;

//            while (true)
//            {
//                UpdateGameState(tankMovement);

//                // Обновляем каждые 5 кадров
//                if (frameCount % 4 == 0)
//                {
//                    RenderGame();
//                }

//                frameCount++;

//                // Устанавливаем задержку на основе времени
//                DateTime currentTime = DateTime.Now;
//                TimeSpan elapsedTime = currentTime - lastFrameTime;

//                // Если прошло меньше 100 мс, задерживаем выполнение
//                if (elapsedTime.TotalMilliseconds < 100)
//                {
//                    System.Threading.Thread.Sleep(100 - (int)elapsedTime.TotalMilliseconds);
//                }

//                lastFrameTime = DateTime.Now; // Обновляем время последнего кадра
//            }
//        }

//        private void UpdateGameState(TankMovement tankController)
//        {
//            if (tankController == null)
//            {
//                throw new InvalidOperationException("tankController не инициализирован.");
//            }

//            if (Console.KeyAvailable)
//            {
//                var key = Console.ReadKey(true).Key;

//                // Обновляем направление танка и перемещение в зависимости от нажатой клавиши
//                UpdateTankDirection(mainPlayerTank, key);

//                switch (key)
//                {
//                    case ConsoleKey.UpArrow:
//                    case ConsoleKey.W: // Для удобства добавили W
//                        tankController.Move(Direction.Up);
//                        break;
//                    case ConsoleKey.DownArrow:
//                    case ConsoleKey.S: // Для удобства добавили S
//                        tankController.Move(Direction.Down);
//                        break;
//                    case ConsoleKey.LeftArrow:
//                    case ConsoleKey.A: // Для удобства добавили A
//                        tankController.Move(Direction.Left);
//                        break;
//                    case ConsoleKey.RightArrow:
//                    case ConsoleKey.D: // Для удобства добавили D
//                        tankController.Move(Direction.Right);
//                        break;
//                    case ConsoleKey.Spacebar:
//                        mainPlayerTank.Shoot(bullets, frameCount); // Указываем текущее время в кадрах
//                        break;
//                }
//            }

//            // Обновляем врагов
//            foreach (var enemy in enemyTanks)
//            {
//                enemy.Update(bullets);
//            }

//            // Обновляем снаряды
//            UpdateBullets();
//        }

//        private void UpdateBullets()
//        {
//            // Обновляем снаряды и удаляем те, которые не могут продолжать движение
//            bullets.RemoveAll(b => !b.UpdateBullet() ||
//                                  (b.BulletX < 0 || b.BulletX >= MapWidth || b.BulletY < 0 || b.BulletY >= MapHeight));

//            // Проверяем попадания снарядов в врагов
//            List<int> bulletsToRemove = new List<int>();
//            List<int> enemiesToRemove = new List<int>();

//            for (int i = 0; i < bullets.Count; i++)
//            {
//                var bullet = bullets[i];

//                for (int j = 0; j < enemyTanks.Count; j++)
//                {
//                    var enemy = enemyTanks[j];
//                    if (bullet.BulletX == enemy.X && bullet.BulletY == enemy.Y)
//                    {
//                        // Помечаем врага и снаряд для удаления
//                        enemiesToRemove.Add(j);
//                        bulletsToRemove.Add(i);
//                        break; // Выходим из цикла после попадания
//                    }
//                }
//            }

//            // Удаляем врагов
//            for (int i = enemiesToRemove.Count - 1; i >= 0; i--)
//            {
//                enemyTanks.RemoveAt(enemiesToRemove[i]);
//            }

//            // Удаляем снаряды
//            for (int i = bulletsToRemove.Count - 1; i >= 0; i--)
//            {
//                bullets.RemoveAt(bulletsToRemove[i]);
//            }
//        }

//        public void UpdateTankDirection(Tank playerTank, ConsoleKey key)
//        {
//            switch (key)
//            {
//                case ConsoleKey.UpArrow:
//                    playerTank.Direction = Direction.Up;
//                    break;
//                case ConsoleKey.DownArrow:
//                    playerTank.Direction = Direction.Down;
//                    break;
//                case ConsoleKey.LeftArrow:
//                    playerTank.Direction = Direction.Left;
//                    break;
//                case ConsoleKey.RightArrow:
//                    playerTank.Direction = Direction.Right;
//                    break;
//            }
//        }

//        private void RenderGame()
//        {
//            Console.SetCursorPosition(0, 0);
//            gameMap.Render(); // Отображаем карту
//            renderer.Render(mainPlayerTank, bullets); // Отображаем основной танк и снаряды

//            // Отображаем врагов
//            foreach (var enemy in enemyTanks)
//            {
//                renderer.RenderEnemyTank(enemy); // Рендеринг врага
//            }
//        }
//    }
//}

//using System;
//using Tanki.Map;

//namespace Tanki.Tanks
//{
//    public class Bullet
//    {
//        public int BulletX { get; set; }
//        public int BulletY { get; set; }
//        public Direction Direction { get; set; }
//        private int updateCooldown; // Количество кадров между обновлениями
//        private int updateTimer; // Таймер обновления
//        private GameMap gameMap; // Ссылка на карту

//        public Bullet(int x, int y, Direction direction, GameMap map, int cooldown = 5)
//        {
//            BulletX = x;
//            BulletY = y;
//            Direction = direction;
//            gameMap = map; // Инициализируем карту
//            updateCooldown = cooldown; // Устанавливаем значение cooldown
//            updateTimer = 0; // Инициализируем таймер
//        }

//        public bool UpdateBullet()
//        {

//            if (gameMap == null)
//            {
//                throw new InvalidOperationException("GameMap is not initialized."); // Исключение, если gameMap не инициализирован
//            }

//            updateTimer++;

//            if (updateTimer >= updateCooldown) // Проверяем, нужно ли обновить позицию
//            {
//                // Проверяем, можно ли двигаться дальше
//                if (!gameMap.CanShootThrough(BulletX, BulletY) ||
//                    BulletX < 0 || BulletX >= gameMap.Width ||
//                    BulletY < 0 || BulletY >= gameMap.Height)
//                {
//                    return false; // Если снаряд не может пройти, возвращаем false для удаления
//                }

//                // Обновляем позицию снаряда
//                switch (Direction)
//                {
//                    case Direction.Up:
//                    case Direction.North:
//                        BulletY--; // Двигаем снаряд вверх
//                        break;
//                    case Direction.Down:
//                    case Direction.South:
//                        BulletY++; // Двигаем снаряд вниз
//                        break;
//                    case Direction.Right:
//                    case Direction.East:
//                        BulletX++; // Двигаем снаряд вправо
//                        break;
//                    case Direction.Left:
//                    case Direction.West:
//                        BulletX--; // Двигаем снаряд влево
//                        break;
//                }
//                updateTimer = 0; // Сбрасываем таймер
//            }
//            return true; // Если снаряд продолжает движение, возвращаем true
//        }
//    }
//}

//using System;
//using System.Collections.Generic;
//using Tanki.Map;

//namespace Tanki.Tanks
//{
//    public abstract class Tank
//    {
//        protected int x;
//        protected int y;
//        public Direction Direction { get; set; }

//        public int X
//        {
//            get => x;
//            set
//            {
//                if (value >= 0 && value < Game.MapWidth) x = value;
//            }
//        }

//        public int Y
//        {
//            get => y;
//            set
//            {
//                if (value >= 0 && value < Game.MapHeight) y = value;
//            }
//        }

//        protected Tank(int x, int y, Direction direction)
//        {
//            X = x;
//            Y = y;
//            Direction = direction;
//        }

//        public void Move(Direction direction)
//        {
//            switch (direction)
//            {
//                case Direction.North:
//                case Direction.Up:
//                    if (Y > 0) Y--;
//                    break;
//                case Direction.South:
//                case Direction.Down:
//                    if (Y < Game.MapHeight - 1) Y++;
//                    break;
//                case Direction.East:
//                case Direction.Right:
//                    if (X < Game.MapWidth - 1) X++;
//                    break;
//                case Direction.West:
//                case Direction.Left:
//                    if (X > 0) X--;
//                    break;
//            }
//        }

//        public abstract void Shoot(List<Bullet> bullets, int cooldown); // Абстрактный метод для стрельбы
//    }
//}