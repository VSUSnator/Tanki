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
            // Логика обновления, если это необходимо
        }

        private void HandleCollisions()
        {
            // Логика обработки столкновений
        }
    }
}