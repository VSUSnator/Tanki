using System;
using Tanki.Map;

namespace Tanki
{
    public class TankEnemy : Tank
    {
        private Random random;
        private GameState gameState;

        public TankEnemy(int x, int y, GameMap map, GameState gameState) : base(x, y, map)
        {
            random = new Random();
            this.gameState = gameState;
        }

        public void Update()
        {
            // Простой алгоритм движения врага
            int dx = random.Next(-1, 2); // Случайное смещение по X (-1, 0, 1)
            int dy = random.Next(-1, 2); // Случайное смещение по Y (-1, 0, 1)

            Move(dx, dy); // Двигаем врага

            // Враг может стрелять с некоторой вероятностью
            if (random.Next(0, 10) < 3) // 30% шанс выстрела
            {
                Bullet bullet = Shoot();
                if (bullet != null)
                {
                    gameState.Bullets.Add(bullet); // Добавляем пулю в список снарядов
                }
            }
        }

        public override void Draw(Renderer renderer)
        {
            // Используем представления из родительского класса
            base.Draw(renderer);
            char[][] tankRepresentation = TankRepresentations[CurrentDirection]; // Получаем представление в зависимости от направления

            for (int i = 0; i < tankRepresentation.Length; i++)
            {
                for (int j = 0; j < tankRepresentation[i].Length; j++)
                {
                    // Убедитесь, что координаты находятся в пределах
                    if (X + j >= 0 && X + j < renderer.width && Y + i >= 0 && Y + i < renderer.height)
                    {
                        renderer.SetPixel(X + j, Y + i, tankRepresentation[i][j], 3); // Используйте другой индекс цвета для врагов
                    }
                }
            }
        }
    }
}