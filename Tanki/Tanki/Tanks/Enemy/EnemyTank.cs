using Tanki.Map;

namespace Tanki
{
    public class TankEnemy : Tank
    {
        public TankEnemy(int x, int y, GameMap map) : base(x, y, map)
        {
            // Инициализация переменных, если это необходимо
            CurrentDirection = Direction.Up; // Устанавливаем начальное направление
        }

        public void Update(GameState gameState)
        {
            if (gameState.PlayerTank != null)
            {
                RotateTowardsPlayer(gameState.PlayerTank); // Поворачиваемся к игроку
                var path = AStar(X, Y, gameState.PlayerTank.X, gameState.PlayerTank.Y);

                if (path != null && path.Count > 0)
                {
                    var nextStep = path.First();
                    Move(nextStep.X - X, nextStep.Y - Y); // Двигаемся к следующему шагу пути

                    // Проверка, может ли враг стрелять
                    if (CanShootAtPlayer(gameState.PlayerTank))
                    {
                        Shoot();
                    }
                }
            }
        }

        private void RotateTowardsPlayer(ITank playerTank)
        {
            if (X < playerTank.X)
                ChangeDirection(Direction.Right);
            else if (X > playerTank.X)
                ChangeDirection(Direction.Left);
            else if (Y < playerTank.Y)
                ChangeDirection(Direction.Down);
            else if (Y > playerTank.Y)
                ChangeDirection(Direction.Up);
        }

        public void Draw(Renderer renderer)
        {
            char[][] tankRepresentation = TankRepresentations[CurrentDirection];
            for (int i = 0; i < tankRepresentation.Length; i++)
            {
                for (int j = 0; j < tankRepresentation[i].Length; j++)
                {
                    renderer.SetPixel(X + j, Y + i, tankRepresentation[i][j], 2);
                }
            }
        }

        private bool CanShootAtPlayer(ITank playerTank)
        {
            // Логика, чтобы определить, находится ли игрок в пределах досягаемости
            // Например, если игрок в пределах 1 клетки от врага
            return Math.Abs(X - playerTank.X) <= 1 && Math.Abs(Y - playerTank.Y) <= 1;
        }

        public List<Node> AStar(int startX, int startY, int targetX, int targetY)
        {
            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            var startNode = new Node(startX, startY);
            var targetNode = new Node(targetX, targetY);

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();

                if (currentNode.X == targetX && currentNode.Y == targetY)
                {
                    return RetracePath(startNode, currentNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor) || !IsValidMove(neighbor.X, neighbor.Y))
                        continue;

                    int newCostToNeighbor = currentNode.GCost + 1; // Расстояние до соседа

                    if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = GetHeuristic(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // Если путь не найден
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            // Добавление соседей (вверх, вниз, влево, вправо)
            if (node.X > 0) neighbors.Add(new Node(node.X - 1, node.Y));
            if (node.X < gameMap.Width - 1) neighbors.Add(new Node(node.X + 1, node.Y));
            if (node.Y > 0) neighbors.Add(new Node(node.X, node.Y - 1));
            if (node.Y < gameMap.Height - 1) neighbors.Add(new Node(node.X, node.Y + 1));

            return neighbors;
        }

        private int GetHeuristic(Node a, Node b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Манхэттенское расстояние
        }

        private bool IsValidMove(int x, int y)
        {
            return x >= 0 && x < gameMap.Width && y >= 0 && y < gameMap.Height && !IsObstacle(x, y);
        }
    }
}