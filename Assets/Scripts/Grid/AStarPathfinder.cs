using System.Collections.Generic;
using Characters.Models;
using Grid;
using UnityEngine;

public static class AStarPathfinder 
{
    private static readonly int[] dx = { 1, -1, 0, 0, 1, -1, 1, -1 };
    private static readonly int[] dy = { 0, 0, 1, -1, 1, 1, -1, -1 };

    private static List<Cell> GetNeighbors(Cell node)
    {
        List<Cell> neighbors = new List<Cell>();

        for (int i = 0; i < dx.Length; i++)
        {
            int newX = node.Coordinates.x + dx[i];
            int newY = node.Coordinates.y + dy[i];

            var cell = BoardManager.TryGetCell(newX, newY);
            if (cell != null)
            {
                neighbors.Add(cell);
            }
        }

        return neighbors;
    }

    public static List<Cell> FindPath(Cell startNode, Cell targetNode)
    {
        List<Cell> openList = new List<Cell>();
        HashSet<Cell> closedList = new HashSet<Cell>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Cell currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                // Path found, reconstruct and use the path.
                return  RetracePath(startNode, targetNode);
            }

            foreach (Cell neighbor in GetNeighbors(currentNode))
            {
                if ((neighbor.IsOccupied && neighbor.Character is EnemyCharacter) || closedList.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private static List<Cell> RetracePath(Cell startNode, Cell endNode)
    {
        List<Cell> path = new List<Cell>();
        Cell currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(Cell nodeA, Cell nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Coordinates.x - nodeB.Coordinates.x);
        int dstY = Mathf.Abs(nodeA.Coordinates.y - nodeB.Coordinates.y);

        return dstX + dstY;
    }
}