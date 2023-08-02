using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Enemy
{
    public class Drone : EnemyCharacter
    {
        [SerializeField] private GameObject shotPrefab;
        [SerializeField] private Transform shotPosition;
        
        private List<Cell> attackHighlightedCells = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();

        private Cell attackTarget;

        private void Start()
        {
            SetIdleAnimation();
        }

        private void SetIdleAnimation()
        {
            Transform body = transform.GetChild(0);
            body.DOLocalMove(body.localPosition - body.up * 0.1f, 2)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

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
            availableTargets = CharacterActions.GetAvailableTargets<PlayerCharacter>(Location, CharacterActions.DirectionType.Diagonal);

            if (availableTargets.Count > 0)
            {
                StartCoroutine(AnimateAttack());
            }

            return false;
        }

        private IEnumerator AnimateAttack()
        {
            attackTarget = GetTarget();
            GetDirectionToTarget(attackTarget);

            foreach (var cell in attackHighlightedCells)
            {
                cell.RedBlink(false, 2);
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(0.6f);
            
            Vector3 difference = attackTarget.Position - Location.Position;
            var rotY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
            DOVirtual.Float(transform.eulerAngles.y, rotY, 1.25f, value =>
                {
                    transform.rotation = Quaternion.Euler(0, value, 0);
                })
                .OnComplete(() =>
                {
                    attackTarget.AttackCell(AttackPonints);
                    CharacterActions.ShootLaser(transform, shotPrefab, shotPosition, attackTarget);
                    Recoil();
                });
        }

        private void Recoil()
        {
            var direction = transform.position - transform.forward * 0.3f;
            transform.DOMove(direction, 0.15f)
                .SetEase(Ease.InFlash)
                .SetLoops(2, LoopType.Yoyo);
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
                    var nextCell = BoardManager.TryGetCell(r += rowDir, c += colDir);
                    if (nextCell != null && nextCell != target)
                    {
                        attackHighlightedCells.Add(nextCell);
                    }
                    else
                    {
                        break;
                    }
                }
                attackHighlightedCells.Add(target);
            }
        }
    }
}
