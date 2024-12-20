//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Tanki.Tanks;
//using Tanki.Map;
//using Tanki;

//namespace Tanki.Tanks
//{
//    public class TankMovement
//    {
//        private Tank tank;
//        private int mapWidth;
//        private int mapHeight;

//        public TankMovement(Tank tank, int mapWidth, int mapHeight)
//        {
//            this.tank = tank;
//            this.mapWidth = mapWidth;
//            this.mapHeight = mapHeight;
//        }

//        public void Move(Direction direction)
//        {
//            switch (direction)
//            {
//                case Direction.North:
//                case Direction.Up:
//                    if (tank.Y > 0) tank.Y--;
//                    break;
//                case Direction.South:
//                case Direction.Down:
//                    if (tank.Y < mapHeight - 1) tank.Y++;
//                    break;
//                case Direction.East:
//                case Direction.Right:
//                    if (tank.X < mapWidth - 1) tank.X++;
//                    break;
//                case Direction.West:
//                case Direction.Left:
//                    if (tank.X > 0) tank.X--;
//                    break;
//            }
//        }
//    }
//}

//namespace Tanki.Tanks
//{
//    public enum Direction
//    {
//        Up,
//        Down,
//        Left,
//        Right,
//        North,
//        South,
//        East,
//        West
//    }
//}

//namespace Tanki
//{
//    public class Renderer
//    {
//        private int mapWidth;
//        private int mapHeight;

//        public Renderer(int mapWidth, int mapHeight)
//        {
//            this.mapWidth = mapWidth;
//            this.mapHeight = mapHeight;
//        }

//        public void Render(Tank playerTank, List<Bullet> bullets)
//        {
//            Console.Clear();

//            // Отображение танка
//            string[] tankSymbols = GetTankSymbols(playerTank.Direction);
//            for (int i = 0; i < tankSymbols.Length; i++)
//            {
//                Console.SetCursorPosition(playerTank.X, playerTank.Y + i);
//                Console.Write(tankSymbols[i]); // Выводим символы танка
//            }

//            // Отображение снарядов
//            foreach (var bullet in bullets)
//            {
//                // Проверка, что снаряд находится в пределах карты
//                if (bullet.BulletX >= 0 && bullet.BulletX < mapWidth && bullet.BulletY >= 0 && bullet.BulletY < mapHeight)
//                {
//                    Console.SetCursorPosition(bullet.BulletX, bullet.BulletY);
//                    Console.Write("*"); // Символ для снаряда
//                }
//            }
//        }

//        private string[] GetTankSymbols(Direction direction)
//        {
//            switch (direction)
//            {
//                case Direction.Up:
//                    return new string[]
//                    {
//                        @"|^|",
//                    };
//                case Direction.Down:
//                    return new string[]
//                    {
//                        @"|v|",
//                    };
//                case Direction.Left:
//                    return new string[]
//                    {
//                        @"<|",
//                    };
//                case Direction.Right:
//                    return new string[]
//                    {
//                        @"|>",
//                    };
//                default:
//                    return new string[] { "  ^  ", " /|\\ ", "=====" }; // По умолчанию
//            }
//        }
//    }
//}

//namespace Tanki
//{
//    namespace Tanki
//    {
//        public class Program
//        {
//            static void Main()
//            {
//                var gameLogic = new Game();
//                gameLogic.Start();
//            }
//        }
//    }

//}

//namespace Tanki.Tanks
//{
//    public class Tank
//    {
//        public int X { get; set; }
//        public int Y { get; set; }
//        public Direction Direction { get; set; }

//        public Tank(int x, int y, Direction direction)
//        {
//            X = x;
//            Y = y;
//            Direction = direction;
//        }

//        public void Shoot(List<Bullet> bullets, int offsetX = 0, int offsetY = 0)
//        {
//            // Определяем координаты появления снаряда в зависимости от направления танка
//            int bulletX = X + offsetX;
//            int bulletY = Y + offsetY;

//            switch (Direction)
//            {
//                case Direction.Up:
//                    bulletX += 1;   
//                    bulletY -= 0;   
//                    break;
//                case Direction.Down:
//                    bulletX += 1;   
//                    bulletY += 1;   
//                    break;
//                case Direction.Left:
//                    bulletX -= 0;   
//                    bulletY += 0;   
//                    break;
//                case Direction.Right:
//                    bulletX += 2;   
//                    bulletY += 0;   
//                    break;
//            }

//            bullets.Add(new Bullet(bulletX, bulletY, Direction));
//        }
//    }
//}

//namespace Tanki.Tanks
//{
//    public class Bullet
//    {
//        public int BulletX { get; set; }
//        public int BulletY { get; set; }
//        public Direction Direction { get; set; }

//        public Bullet(int x, int y, Direction direction)
//        {
//            BulletX = x;
//            BulletY = y;
//            Direction = direction;
//        }

//        public void UpdateBullet()
//        {
//            switch (Direction)
//            {
//                case Direction.North:
//                case Direction.Up:
//                    BulletY--; // Двигаем снаряд вверх
//                    break;
//                case Direction.South:
//                case Direction.Down:
//                    BulletY++; // Двигаем снаряд вниз
//                    break;
//                case Direction.East:
//                case Direction.Right:
//                    BulletX++; // Двигаем снаряд вправо
//                    break;
//                case Direction.West:
//                case Direction.Left:
//                    BulletX--; // Двигаем снаряд влево
//                    break;
//            }
//        }
//    }
//}

//namespace Tanki
//{
//    public class Game
//    {
//        private Tank mainPlayerTank; // Основной танк игрока
//        private TankMovement tankController; // Контроллер движения танка
//        private Renderer renderer; // Объект рендерера
//        private List<Bullet> bullets = new List<Bullet>(); // Список снарядов
//        private int mapWidth = 50; // Ширина карты
//        private int mapHeight = 24; // Высота карты
//        private GameMap gameMap; // Объект карты
//        private int frameCount = 0;

//        public Game()
//        {
//            mainPlayerTank = new Tank(mapWidth / 2, mapHeight / 2, Direction.Up);
//            tankController = new TankMovement(mainPlayerTank, mapWidth, mapHeight);
//            renderer = new Renderer(mapWidth, mapHeight);
//            gameMap = new GameMap(mapWidth, mapHeight);
//            gameMap.Render();
//        }

//        public void Start()
//        {
//            while (true)
//            {
//                UpdateGameState();

//                // Обновляем каждые 5 кадров
//                if (frameCount % 5 == 0)
//                {
//                    RenderGame();
//                }

//                frameCount++;
//                System.Threading.Thread.Sleep(100); // Пауза между кадрами
//            }

//        }

//        private void UpdateGameState()
//        {
//            if (Console.KeyAvailable)
//            {
//                var key = Console.ReadKey(true).Key;

//                // Обновляем направление танка
//                UpdateTankDirection(mainPlayerTank, key);

//                switch (key)
//                {
//                    case ConsoleKey.UpArrow:
//                        tankController.Move(Direction.North); // Двигаем танк вверх
//                        break;
//                    case ConsoleKey.DownArrow:
//                        tankController.Move(Direction.South); // Двигаем танк вниз
//                        break;
//                    case ConsoleKey.LeftArrow:
//                        tankController.Move(Direction.West); // Двигаем танк влево
//                        break;
//                    case ConsoleKey.RightArrow:
//                        tankController.Move(Direction.East); // Двигаем танк вправо
//                        break;
//                    case ConsoleKey.Spacebar:
//                        mainPlayerTank.Shoot(bullets); // Стрельба
//                        break;
//                }

//                // Обновляем снаряды
//                for (int i = bullets.Count - 1; i >= 0; i--)
//                {
//                    var bullet = bullets[i];
//                    bullet.UpdateBullet();
//                    if (bullet.BulletX < 0 || bullet.BulletX >= mapWidth || bullet.BulletY < 0 || bullet.BulletY >= mapHeight)
//                    {
//                        bullets.RemoveAt(i); // Удаляем, если снаряд вышел за пределы карты
//                    }
//                }
//            }
//        }

//        public void UpdateTankDirection(Tank playerTank, ConsoleKey key)
//        {
//            switch (key)
//            {
//                case ConsoleKey.UpArrow:
//                    playerTank.Direction = Direction.Up; // Убедитесь, что это соответствует вашему перечислению
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
//            gameMap.Render();
//            renderer.Render(mainPlayerTank, bullets); // Исправлено на mainPlayerTank
//        }
//    }
//}