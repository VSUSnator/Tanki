using System.Collections.Generic;
using Tanki.Map;

namespace Tanki
{
    public class GameState
    {
        public Tank Tank { get; private set; }
        public List<Bullet> Bullets { get; private set; }
        public GameMap GameMap { get; private set; } // Добавьте это свойство
        public bool IsGameActive { get; private set; }

        public GameState(string mapFilePath)
        {
            Tank = new Tank(5, 10);
            Bullets = new List<Bullet>();
            GameMap = new GameMap(mapFilePath); // Инициализируем карту
            IsGameActive = false;
        }

        public void StartGame()
        {
            IsGameActive = true;
            // Здесь можно добавить дополнительную логику инициализации
        }

        public void EndGame()
        {
            IsGameActive = false;
            // Здесь можно добавить логику завершения, например, сохранение результатов
        }

        public void AddBullet()
        {
            var bullet = Tank.Shoot();
            Bullets.Add(bullet);
        }

        public void Update()
        {
            if (IsGameActive)
            {
                UpdateBullets(); // Обновляем снаряды
            }
        }

        public string GetMapSymbol(int x, int y)
        {
            return GameMap.GetMapSymbol(x, y).ToString(); // Преобразуем char в string
        }

        public void UpdateBullets()
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Move();
                if (Bullets[i].X < 0 || Bullets[i].X >= 20 || Bullets[i].Y < 0 || Bullets[i].Y >= 15) // Если снаряд вышел за пределы экрана
                {
                    Bullets.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}