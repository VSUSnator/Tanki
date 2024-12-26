using Tanki.Map;

namespace Tanki
{
    public class TankEnemy : Tank
    {
        public TankEnemy(int x, int y, GameMap map) : base(x, y, map) { }

        public void Update()
        {
            // Здесь можно реализовать логику ИИ для врага
        }
    }
}