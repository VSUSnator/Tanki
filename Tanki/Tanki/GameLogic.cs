using Tanki.Map;

namespace Tanki
{
    public class GameLogic
    {
        private GameState gameState;

        public GameLogic(GameState state)
        {
            gameState = state;
        }

        public void Update()
        {
            if (gameState.IsGameActive)
            {
                HandleCollisions();
                // Здесь можно добавить другие логические обновления
            }
        }

        private void HandleCollisions()
        {
            for (int i = 0; i < gameState.Bullets.Count; i++)
            {
                var bullet = gameState.Bullets[i];
                // Проверка столкновения с танком игрока
                if (IsColliding(bullet, gameState.PlayerTank))
                {
                    // Логика обработки столкновения
                    gameState.Bullets.RemoveAt(i);
                    i--; // Уменьшаем индекс, чтобы не пропустить следующий элемент
                }
            }
        }

        private bool IsColliding(Bullet bullet, Tank tank)
        {
            // Простая проверка на столкновение (например, по координатам)
            return bullet.X == tank.X && bullet.Y == tank.Y;
        }
    }
}