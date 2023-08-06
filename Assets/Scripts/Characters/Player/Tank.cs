using System.Collections.Generic;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;
using UnityEngine.Android;

namespace Characters.Player
{
    public class Tank : PlayerCharacter
    {
        [SerializeField] private GameObject shotPrefab;
        [SerializeField] private Transform shotPosition;
        
        private List<Cell> availableMovePositions = new List<Cell>();
        private List<Cell> attackHighlightedCells = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();
        
        // Define the directions the queen can move (horizontally, vertically, and diagonally)
        private readonly int[] dirR = { 0, 0, 1, -1, 1, -1, 1, -1 };
        private readonly int[] dirC = { 1, -1, 0, 0, 1, 1, -1, -1 };
        
        private Animator anim;
        private int paramRun, paramShoot, paramDie;
        private readonly int maxMoveDistance = 3;
        private Cell attackTarget;
        
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
           
            anim = GetComponentInChildren<Animator>();
            paramShoot = Animator.StringToHash("shoot");
            paramRun = Animator.StringToHash("run");
            paramDie = Animator.StringToHash("death");
        }

        public override void GetMovementCells()
        {
            var psn = Location.Coordinates;

            availableMovePositions = new List<Cell>();

            for (int i = 0; i < dirR.Length; i++)
            {
                int r = psn.x;
                int c = psn.y;

                for (int moveDistance = 1; moveDistance <= maxMoveDistance; moveDistance++)
                {
                    r += dirR[i];
                    c += dirC[i];

                    var cell = BoardManager.TryGetCell(r, c);
                    if (cell != null)
                    {
                        if (!cell.IsOccupied) availableMovePositions.Add(cell);
                        else
                        {
                            // The position is blocked by another piece, so we stop searching in this direction
                            break;
                        }
                    }
                    else
                    {
                        // The position is outside the chessboard, so we stop searching in this direction
                        break;
                    }
                }
            }
            
            CanMove = false;
            foreach (var cell in availableMovePositions)
            {
                if (cell != null && !cell.IsOccupied)
                {
                    CanMove = !HasMoved;
                    break;
                }
            }
        }
        public override void ShowMoveLocations()
        {
            HideLocations();
            GetMovementCells();
            
            foreach (var cell in availableMovePositions)
            {
                if(cell != null && !cell.IsOccupied) cell.GreenHighlight();
            }
        }
        
        public override void GetAttackTargets()
        {
            availableTargets = CharacterActions.GetAvailableTargets<EnemyCharacter>(Location, CharacterActions.DirectionType.Orthogonal);

            CanAttack = availableTargets.Count > 0 && !HasAttacked;
        }
        
        public override void ShowAttackLocations()
        {
            HideLocations();
            attackHighlightedCells = new List<Cell>();
            availableTargets = CharacterActions.GetAvailableTargets<EnemyCharacter>(Location, CharacterActions.DirectionType.Orthogonal, attackHighlightedCells);
            
            if (availableTargets.Count > 0)
            {
                CanAttack = !HasAttacked;
                AnimateAttackLocations();
            }
            else CanAttack = false;
        }
        
        private void AnimateAttackLocations()
        {
            foreach (var cell in attackHighlightedCells)
            {
                if(availableTargets.Contains(cell)) cell.RedBlink(true, -1);
                else cell.RedBlink(false, -1);
            }
        }

        protected override void HideLocations()
        {
            foreach (var cell in availableMovePositions)
            {
                if(cell != null) cell.DisableHighlight();
            }
            
            foreach (var cell in attackHighlightedCells)
            {
                cell.DisableHighlight();
            }
        }

        protected override void Move(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            transform.LookAt(cell.Position);
            
            RegisterMove(cell);
            
            var pos = transform.position;
            anim.SetBool(paramRun, true);
            float speed = Vector3.Distance(transform.position, cell.Position) * 0.6f;
            DOVirtual.Vector3(pos, cell.Position, speed, value => transform.position = value)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    anim.SetBool(paramRun, false);
                    if (!HasAttacked && Bm.IsCurrentlySelected(this)) ShowAttackLocations();
                    Bm.ShowSelectOverlay(this);
                });
        }
        
        protected override void Attack(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            RegisterAttack();
            
            Vector3 difference = cell.Position - Location.Position;
            var rotY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
            var yAngle = transform.eulerAngles.y;
            if (yAngle - rotY > 180) rotY += 360;
            DOVirtual.Float(yAngle, rotY, 1.25f,
                    value => transform.rotation = Quaternion.Euler(0, value, 0))
                .OnComplete(() =>
                {
                    float delay = CharacterActions.GetManhattanDistance(cell, Location);
                    attackTarget = cell;
                    Invoke(nameof(ApplyDamage), delay * 0.08f);
                    
                    anim.SetTrigger(paramShoot);
                    CharacterActions.ShootLaser(transform, shotPrefab, shotPosition, cell);
                    Bm.ShowSelectOverlay(this);
                });
        }

        private void ApplyDamage()
        {
            attackTarget.AttackCell(AttackPonints);
            BoardManager.Instance.CheckActions();
        }
        
        public override void Die()
        {
            anim.SetTrigger(paramDie);
            base.Die();
        }

        public override void GetDamaged()
        {
            GameUIController.Instance.UpdatePlayerDisplay(this);
            // TODO: Add animation for getting damaged
        }
    }
}
