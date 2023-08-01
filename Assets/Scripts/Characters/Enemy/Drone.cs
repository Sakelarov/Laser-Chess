using System.Collections.Generic;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Enemy
{
    public class Drone : EnemyCharacter
    {
        private List<Cell> attackHighlightedCells = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();
        
        // Define the directions the drone can attack
        private int[] dr = { 1, -1, 1, -1 };
        private int[] dc = { 1, 1, -1, -1 };
        
        public override bool TryMove()
        {
            var psn = Location.Coordinates;
            
            var targetCell = BoardManager.TryGetCell(psn.x - 1, psn.y);

            if (targetCell != null && !targetCell.IsOccupied)
            {
                Move(targetCell);
                return true;
            }

            return false;
        }

        private void Move(Cell cell)
        {
            transform.LookAt(cell.Position);
            
            HasMoved = true;
           
            Location.SetCharacter(null);
            Location = cell;
            cell.SetCharacter(this);
            
            if(cell.Coordinates.x == 0) Invoke(nameof(EnemyWin), 1.5f);
           
            var pos = transform.position;
            DOVirtual.Vector3(pos, cell.Position, 1, value => transform.position = value)
                .SetEase(Ease.InOutCubic);
        }

        private void EnemyWin()
        {
            Debug.Log("Enemies Win!");
        }

        public override bool TryAttack()
        {
            GetAvailableTargets();

            if (availableTargets.Count > 0)
            {
                
            }

            return false;
        }

        private void AnimateAttackPath()
        {
            GetDirectionToTarget(GetTarget());
            
            
        }

        private Cell GetTarget()
        {
            float closestDistance = float.MaxValue;
            Cell closestTarget = null;
            foreach (var target in availableTargets)
            {
                var distance = (Vector3.Distance(Location.Position, target.Position));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        private void GetDirectionToTarget(Cell target)
        {
            if (target != null)
            {
                attackHighlightedCells = new List<Cell>();
                int rowDir = (target.Coordinates.x - Location.Coordinates.x) > 0 ? 1 : -1;
                int colDir = (target.Coordinates.y - Location.Coordinates.y) > 0 ? 1 : -1;
                int r = Location.Coordinates.x;
                int c = Location.Coordinates.y;

                while (true)
                {
                    var prevCell = BoardManager.TryGetCell(r += rowDir, c += colDir);
                    if (prevCell != null && prevCell != target)
                    {
                        attackHighlightedCells.Add(prevCell);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void GetAvailableTargets()
        {
            var psn = Location.Coordinates;
            availableTargets = new List<Cell>();
            
            for (int i = 0; i < dr.Length; i++)
            {
                int r = psn.x;
                int c = psn.y;

                while (true)
                {
                    r += dr[i];
                    c += dc[i];

                    var cell = BoardManager.TryGetCell(r, c);
                    if (cell != null)
                    {
                        if (cell.IsOccupied)
                        {
                            availableTargets.Add(cell);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
