using System;
using System.Collections.Generic;
using Tanki.Map;
using Tanki.Tanks;
using Tanki.Tanks.Player;

namespace Tanki
{
    public class InputHandler
    {
        private readonly PlayerTank playerTank;
        private readonly TankMovement tankMovement;

        // Делегат для события стрельбы
        public delegate void ShootEventHandler(List<Bullet> bullets, int frameCount);

        // Событие, которое будет вызываться при стрельбе
        public event ShootEventHandler OnShoot;

        private readonly Dictionary<ConsoleKey, Direction> keyDirectionMap = new Dictionary<ConsoleKey, Direction>
        {
            { ConsoleKey.UpArrow, Direction.Up },
            { ConsoleKey.W, Direction.Up },
            { ConsoleKey.DownArrow, Direction.Down },
            { ConsoleKey.S, Direction.Down },
            { ConsoleKey.LeftArrow, Direction.Left },
            { ConsoleKey.A, Direction.Left },
            { ConsoleKey.RightArrow, Direction.Right },
            { ConsoleKey.D, Direction.Right }
        };

        public InputHandler(PlayerTank tank, TankMovement movement)
        {
            playerTank = tank;
            tankMovement = movement;
        }

        public void ProcessInput(IReadOnlyList<Bullet> bullets, int frameCount)
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (keyDirectionMap.TryGetValue(key, out var direction))
                {
                    tankMovement.Move(direction);
                    playerTank.Direction = direction; // Устанавливаем направление танка
                }
                else if (key == ConsoleKey.Spacebar)
                {
                    // Создаем новый список на основе IReadOnlyList
                    var bulletsList = new List<Bullet>(bullets);
                    // Вызываем событие OnShoot с новым списком
                    OnShoot?.Invoke(bulletsList, frameCount);
                }
            }
        }
    }
}