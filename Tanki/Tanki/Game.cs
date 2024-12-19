using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki.Tanks;

namespace Tanki
{
    public class Game
    {
        private Map map;
        private Tank playerTank;
        private List<Tank> enemyTanks;

        public void Start()
        {
            map = new Map();
            playerTank = new Tank();
            enemyTanks = new List<Tank>();

            // Инициализация врагов и начало игрового цикла
            while (true)
            {
                Update();
                Render();
            }
        }

        private void Update()
        {
            // Обработка ввода игрока
            // Движение и стрельба врагов
        }

        private void Render()
        {
            // Отображение карты и танков
        }
    }
}
