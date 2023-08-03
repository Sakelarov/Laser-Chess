using System.Collections.Generic;
using Characters.Models;
using Grid;
using UnityEngine;

namespace Characters.Enemy
{
    public class Dreadnought : EnemyCharacter
    {
        private List<Cell> closestPlayerCharacters = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();
        private Cell[] adjacentCells = new Cell[8];

        private Cell topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left;

        private int minDistance = -1;
        
        public override bool TryMove()
        {
            Bm.ForEachCell(CompareClosestPlayer);

            if (closestPlayerCharacters.Count == 1)
            {
                // TODO: move towards the only player unit detected
            }
            else if (closestPlayerCharacters.Count > 1)
            {
                
            }
            return false;
        }

        private void CompareClosestPlayer(Cell cell)
        {
            if (cell.IsOccupied && cell.Character is PlayerCharacter)
            {
                var psn = Location.Coordinates;
                int distance = Mathf.Max(Mathf.Abs(psn.x - cell.Coordinates.x), Mathf.Abs(psn.y - cell.Coordinates.y));
                if (minDistance == -1 || distance == minDistance)
                {
                    minDistance = distance;
                    closestPlayerCharacters.Add(cell);
                }
                else if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlayerCharacters.Clear();
                    closestPlayerCharacters.Add(cell);
                }
            }
        }

        private void GetAdjacentCells()
        {
            var psn = Location.Coordinates;

            adjacentCells[0] = BoardManager.TryGetCell(psn.x + 1, psn.y);
            adjacentCells[1] = BoardManager.TryGetCell(psn.x + 1, psn.y + 1);
            adjacentCells[2] = BoardManager.TryGetCell(psn.x, psn.y + 1);
            adjacentCells[3] = BoardManager.TryGetCell(psn.x - 1, psn.y + 1);
            adjacentCells[4] = BoardManager.TryGetCell(psn.x - 1, psn.y);
            adjacentCells[5] = BoardManager.TryGetCell(psn.x - 1, psn.y - 1);
            adjacentCells[6] = BoardManager.TryGetCell(psn.x, psn.y - 1);
            adjacentCells[7] = BoardManager.TryGetCell(psn.x + 1, psn.y - 1);
        }

        public override bool TryAttack()
        {
            return false;
        }
    }
}
