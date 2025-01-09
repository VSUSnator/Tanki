using System;
using System.Collections.Generic;
using Tanki.Map;
using static Tanki.Tank;

namespace Tanki
{
    public class TankEnemy : Tank
    {
        private GameState gameState;
        private List<Node> path; // Список узлов пути
        private int currentTargetIndex; // Индекс текущей цели в пути
        private HashSet<Node> visitedNodes; // Множество посещенных узлов

        private TimeSpan lastMoveTime; // Время с последнего движения
        private TimeSpan moveInterval = TimeSpan.FromMilliseconds(500); // Интервал между движениями

        public TankEnemy(int x, int y, GameMap map, GameState gameState) : base(x, y, map)
        {
            this.gameState = gameState;
            path = new List<Node>();
            currentTargetIndex = 0;
            visitedNodes = new HashSet<Node>();
            lastMoveTime = TimeSpan.Zero; // Инициализация времени
        }

        public void Update(TimeSpan gameTime)
        {
            lastMoveTime += gameTime; // Увеличиваем время с последнего движения

            if (lastMoveTime < moveInterval) return; // Не достаточно времени, чтобы двигаться
            lastMoveTime = TimeSpan.Zero; // Сбрасываем время после движения

            if (path.Count == 0 || currentTargetIndex >= path.Count)
            {
                Node target = FindClosestPlayerNode();
                path = AStar(new Node(X, Y), target);
                currentTargetIndex = 0;
                visitedNodes.Clear(); // Очищаем посещенные узлы при обновлении пути
            }

            if (currentTargetIndex < path.Count)
            {
                Node targetNode = path[currentTargetIndex];

                if (!CanMoveTo(targetNode))
                {
                    TurnAround(); // Разворот при столкновении
                    path = AStar(new Node(X, Y), FindClosestPlayerNode()); // Пересчитываем путь
                    currentTargetIndex = 0; // Сбрасываем индекс
                    return;
                }

                MoveTo(targetNode);
            }
            else
            {
                MoveInCorridor();
            }
        }

        private void MoveTo(Node targetNode)
        {
            int dx = targetNode.X - X;
            int dy = targetNode.Y - Y;

            // Двигаемся на 1 шаг в сторону цели
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                dx = Math.Sign(dx);
                dy = 0;
            }
            else
            {
                dy = Math.Sign(dy);
                dx = 0;
            }

            if (CanMove(dx, dy) && !visitedNodes.Contains(targetNode))
            {
                Move(dx, dy);
                visitedNodes.Add(new Node(X, Y));

                if (X == targetNode.X && Y == targetNode.Y)
                {
                    currentTargetIndex++;
                }
            }
        }

        private void MoveInCorridor()
        {
            int[] directions = { 1, -1 }; // Вправо и влево
            foreach (var dir in directions)
            {
                if (CanMove(dir, 0))
                {
                    Move(dir, 0);
                    return;
                }
            }

            foreach (var dir in directions)
            {
                if (CanMove(0, dir))
                {
                    Move(0, dir);
                    return;
                }
            }
        }

        private void TurnAround()
        {
            // Логика поворота на 180 градусов может быть здесь
            CurrentDirection = (Direction)(((int)CurrentDirection + 2) % 4);
        }

        private Node FindClosestPlayerNode()
        {
            return new Node(gameState.PlayerTank.X, gameState.PlayerTank.Y);
        }

        private List<Node> AStar(Node start, Node goal)
        {
            Queue<Node> openList = new Queue<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            openList.Enqueue(start);

            while (openList.Count > 0)
            {
                Node currentNode = openList.Dequeue();
                if (currentNode.Equals(goal))
                {
                    return ReconstructPath(currentNode);
                }

                closedList.Add(currentNode);

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor) || visitedNodes.Contains(neighbor) || !CanMoveTo(neighbor))
                    {
                        continue;
                    }

                    if (!openList.Contains(neighbor))
                    {
                        neighbor.Parent = currentNode;
                        openList.Enqueue(neighbor);
                    }
                }
            }

            return new List<Node>(); // Возвращаем пустой список, если путь не найден
        }

        private List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>
            {
                new Node(node.X + 1, node.Y), // Вправо
                new Node(node.X - 1, node.Y), // Влево
                new Node(node.X, node.Y + 1), // Вниз
                new Node(node.X, node.Y - 1)  // Вверх
            };

            // Фильтруем соседей, чтобы оставлять только допустимые
            neighbors.RemoveAll(n => !IsInBounds(n.X, n.Y));
            return neighbors;
        }

        private int GetCost(Node a, Node b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Манхэттенское расстояние
        }

        private List<Node> ReconstructPath(Node currentNode)
        {
            List<Node> totalPath = new List<Node>();

            while (currentNode != null)
            {
                totalPath.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            totalPath.Reverse();
            return totalPath;
        }

        private bool CanMoveTo(Node targetNode)
        {
            // Проверяем, находится ли узел в пределах карты
            if (!IsInBounds(targetNode.X, targetNode.Y))
            {
                return false;
            }

            // Проверяем, является ли целевая позиция препятствием
            if (IsObstacle(targetNode.X, targetNode.Y))
            {
                return false;
            }

            // Проверяем дополнительные клетки, если танк больше 1x1
            if (gameMap.GetMapSymbol(targetNode.X + 1, targetNode.Y) is '#' or 'W' or 'X' || // Право
                gameMap.GetMapSymbol(targetNode.X, targetNode.Y + 1) is '#' or 'W' or 'X' || // Вниз
                gameMap.GetMapSymbol(targetNode.X + 1, targetNode.Y + 1) is '#' or 'W' or 'X') // Право вниз
            {
                return false;
            }

            return true; // Если все проверки пройдены, движение возможно
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < gameMap.Width && y >= 0 && y < gameMap.Height;
        }

        protected new bool IsObstacle(int x, int y)
        {
            return gameMap.GetMapSymbol(x, y) is '#' or 'W' or 'X';
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            char[][] tankRepresentation = TankRepresentations[CurrentDirection];

            for (int i = 0; i < tankRepresentation.Length; i++)
            {
                for (int j = 0; j < tankRepresentation[i].Length; j++)
                {
                    if (X + j >= 0 && X + j < renderer.width && Y + i >= 0 && Y + i < renderer.height)
                    {
                        renderer.SetPixel(X + j, Y + i, tankRepresentation[i][j], 3);
                    }
                }
            }
        }
    }

    public class Node
    {
        public int X { get; }
        public int Y { get; }
        public Node? Parent { get; set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
            Parent = null;
        }

        public override bool Equals(object obj)
        {
            return obj is Node other && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return (X, Y).GetHashCode();
        }
    }
}