using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;
using Cell = Grid.Cell;

namespace Characters.Enemy
{
    public class Dreadnought : EnemyCharacter
    {
        [SerializeField] private ParticleSystem sparks;
        [SerializeField] private ParticleSystem flameThrower;
        private List<Cell> closestPlayerCharacters = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();
        private List<Cell> cellsAtDistance1 = new List<Cell>();
        private List<Cell> cellsAtDistance2 = new List<Cell>();

        private Animator anim;
        private int paramWalk, paramAttack, paramDeath, paramDamage;

        private int minDistance = -1;
        
        private readonly float moveDuration = 1;
        private readonly float attackDuration = 3f;
        private readonly float blinkDelay = 0.2f;
        private readonly float attackDelay = 1f;
        
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            
            anim = GetComponentInChildren<Animator>();
            paramWalk = Animator.StringToHash("walk");
            paramAttack = Animator.StringToHash("attack");
            paramDeath = Animator.StringToHash("death");
            paramDamage = Animator.StringToHash("damage");
        }
        
        public override bool TryMove(ref float delay)
        {
            cellsAtDistance2 = CharacterActions.GetCellsInManhattanDistance(Location, 2);
            
            List<Cell> enemies = new List<Cell>();
            foreach (var cell in cellsAtDistance2)
            {
                if (cell.IsOccupied && cell.Character is PlayerCharacter) enemies.Add(cell);
            }

            if (enemies.Count == 0) // No enemy detected in range. Find all enemies and move towards closest
            {
                return TryMoveToClosestEnemy(ref delay);
            }
            
            if (enemies.Count == 1) // Only one enemy detected in range. If the enemy is already in attack distance don't move
            {
                if (CharacterActions.GetManhattanDistance(enemies[0], Location) == 1) return false;
                
                return MoveToEnemy(ref delay, enemies[0]);
            }
            
            // more than one enemy detected. Trying to reach as many enemies as possible in order to damage them all
            return TryMoveToMoreEnemies(ref delay);
        }

        private bool TryMoveToMoreEnemies(ref float delay)
        {
            // Find all Cells the Dreadnought can move to
            cellsAtDistance1 = CharacterActions.GetCellsInManhattanDistance(Location, 1);
            List<Cell> movableCells = new List<Cell>();
            foreach (var cell in cellsAtDistance1)
            {
                if(!cell.IsOccupied) movableCells.Add(cell);
            }

            // Get all enemies that will be in attack range after Dreadnought moves to that location
            Dictionary<Cell, int> enemiesInRange = new Dictionary<Cell, int>();
            foreach (var cell in movableCells)
            {
                List<Cell> adjacentCells = CharacterActions.GetCellsInManhattanDistance(cell, 1);
                foreach (var c in adjacentCells)
                {
                    if (c.IsOccupied && c.Character is PlayerCharacter)
                    {
                        if (enemiesInRange.ContainsKey(cell)) enemiesInRange[cell] += 1;
                        else enemiesInRange.Add(cell, 1);
                    }
                }
            }

            // Filter all locations to get the one target reaching to as many enemies as possible
            var target = enemiesInRange.OrderByDescending(kvp => kvp.Value).FirstOrDefault(x => x.Value > 0);

            Move(target.Key);
            delay = moveDuration;
            return true;
        }

        private bool TryMoveToClosestEnemy(ref float delay)
        {
            minDistance = -1;
            closestPlayerCharacters.Clear();
            Bm.ForEachCell(CompareClosestPlayer);

            if (closestPlayerCharacters.Count == 1) // If there is only one enemy Move towards it.
            {
                return MoveToEnemy(ref delay, closestPlayerCharacters[0]);
            }
            
            if (closestPlayerCharacters.Count > 1) // If more than one enemy then move towards the one with least hp
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
                        delay = moveDuration;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool MoveToEnemy(ref float delay, Cell target)
        {
            var path = AStarPathfinder.FindPath(Location, target);

            if (path != null && path.Count > 0)
            {
                Move(path[0]);
                delay = moveDuration;
                return true;
            }
                
            return false;
        }
        
        private void Move(Cell cell)
        {
            RegisterMove(cell);
            
            anim.SetBool(paramWalk, true);
            var pos = transform.position;
            DOVirtual.Vector3(pos, cell.Position, moveDuration, value => transform.position = value)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() => anim.SetBool(paramWalk, false));
        }

        private void CompareClosestPlayer(Cell cell)
        {
            if (cell.IsOccupied && cell.Character is PlayerCharacter)
            {
                var psn = Location.Coordinates;
                int manhattanDistance = Mathf.Max(Mathf.Abs(psn.x - cell.Coordinates.x), Mathf.Abs(psn.y - cell.Coordinates.y));
                if (minDistance == -1 || manhattanDistance == minDistance)
                {
                    minDistance = manhattanDistance;
                    closestPlayerCharacters.Add(cell);
                }
                else if (manhattanDistance < minDistance)
                {
                    minDistance = manhattanDistance;
                    closestPlayerCharacters.Clear();
                    closestPlayerCharacters.Add(cell);
                }
            }
        }

        public override bool TryAttack(ref float delay)
        {
            cellsAtDistance1 = CharacterActions.GetCellsInManhattanDistance(Location, 1);

            foreach (var cell in cellsAtDistance1)
            {
                if (cell.IsOccupied && cell.Character is PlayerCharacter)
                {
                    StartCoroutine(AnimateAttack());
                    
                    delay = Cell.BlinkDuration + attackDelay + attackDuration;
                    delay += blinkDelay * cellsAtDistance1.Count;
                    
                    return true;
                }
            }
            return false;
        }

        private IEnumerator AnimateAttack()
        {
            foreach (var cell in cellsAtDistance1)
            {
                cell.RedBlink(false, 2);
                yield return new WaitForSeconds(blinkDelay);
            }
            
            yield return new WaitForSeconds(attackDelay);

            HasAttacked = true;
            portrait.DisableAttackIndicator();
            GameUIController.Instance.UpdateEnemyInfo(this);
            
            anim.SetBool(paramAttack, true);
            flameThrower.Play();
            var yAngle = transform.eulerAngles.y;
            DOVirtual.Float(yAngle, yAngle + 360, attackDuration, value =>
                {
                    transform.rotation = Quaternion.Euler(0, value, 0);
                })
                .OnComplete(() =>
                {
                    foreach (var cell in cellsAtDistance1)
                    {
                        if (cell.IsOccupied && cell.Character is PlayerCharacter ch)
                        {
                            cell.AttackCell(AttackPonints);
                        }
                    }
                    flameThrower.Stop();
                    anim.SetBool(paramAttack, false);
                });
        }
        
        public override void Die()
        {
            sparks.Play();
            anim.SetTrigger(paramDeath);
            base.Die();
        }

        public override void GetDamaged()
        {
            GameUIController.Instance.UpdateEnemyInfo(this);
            sparks.Play();
            anim.SetTrigger(paramDamage);
        }
    }
}
