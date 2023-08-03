using System.Collections;
using System.Collections.Generic;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Enemy
{
    public class Dreadnought : EnemyCharacter
    {
        [SerializeField] private ParticleSystem flameThrower;
        private List<Cell> closestPlayerCharacters = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();
        private Cell[] adjacentCells = new Cell[8];

        private Cell topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left;

        private Animator anim;
        private int paramWalk, paramAttack, paramDeath, paramDamage;

        private int minDistance = -1;
        
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            
            anim = GetComponentInChildren<Animator>();
            paramWalk = Animator.StringToHash("walk");
            paramAttack = Animator.StringToHash("attack");
            paramDeath = Animator.StringToHash("death");
            paramDamage = Animator.StringToHash("damage");
        }
        
        public override bool TryMove()
        {
            Bm.ForEachCell(CompareClosestPlayer);

            if (closestPlayerCharacters.Count == 1)
            {
                var path = AStarPathfinder.FindPath(Location, closestPlayerCharacters[0]);

                if (path != null && path.Count > 0)
                {
                    Move(path[0]);
                    return true;
                }
                
                return false;
            }
            else if (closestPlayerCharacters.Count > 1)
            {
                var minHealth = int.MaxValue;
                Cell targetPlayer = null;
                foreach (var cell in closestPlayerCharacters)
                {
                    int playerHealth = cell.Character.HealthPoints;
                    if (cell.Character.HealthPoints < minHealth)
                    {
                        minHealth = playerHealth;
                        targetPlayer = cell;
                    }
                }

                if (targetPlayer != null)
                {
                    var path = AStarPathfinder.FindPath(Location, targetPlayer);

                    if (path != null && path.Count > 0)
                    {
                        Move(path[0]);
                        return true;
                    }
                }
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
            
            anim.SetBool(paramWalk, true);
            var pos = transform.position;
            DOVirtual.Vector3(pos, cell.Position, 1, value => transform.position = value)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() => anim.SetBool(paramWalk, false));
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
            GetAdjacentCells();

            foreach (var cell in adjacentCells)
            {
                if (cell.IsOccupied && cell.Character is PlayerCharacter)
                {
                    StartCoroutine(AnimateAttack());
                    return true;
                }
            }
            return false;
        }

        private IEnumerator AnimateAttack()
        {
            foreach (var cell in adjacentCells)
            {
                cell.RedBlink(false, 2);
                yield return new WaitForSeconds(0.2f);
            }
            
            yield return new WaitForSeconds(0.6f);
            
            anim.SetBool(paramAttack, true);
            flameThrower.Play();
            var yAngle = transform.eulerAngles.y;
            DOVirtual.Float(yAngle, yAngle - 360, 2.5f, value =>
                {
                    transform.rotation = Quaternion.Euler(0, value, 0);
                })
                .OnComplete(() =>
                {
                    foreach (var cell in adjacentCells)
                    {
                        cell.AttackCell(AttackPonints);
                    }
                    flameThrower.Stop();
                    anim.SetBool(paramAttack, false);
                });
        }
    }
}
