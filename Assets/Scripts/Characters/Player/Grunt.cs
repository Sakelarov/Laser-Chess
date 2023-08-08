using System.Collections.Generic;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Player
{
    public class Grunt : PlayerCharacter
    {
        [SerializeField] private GameObject shotPrefab;
        [SerializeField] private Transform shotPosition;
        
        private List<Cell> attackHighlightedCells = new List<Cell>();
        private List<Cell> availableTargets = new List<Cell>();
        
        private Cell[] availableMovePositions = new Cell[4];

        private Animator anim;
        private int paramSpeed, paramAiming, paramReload, paramShoot, paramDeath;

        private Cell attackTarget;
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
           
            anim = GetComponentInChildren<Animator>();
            paramSpeed = Animator.StringToHash("Speed");
            paramAiming = Animator.StringToHash("Aiming");
            paramReload = Animator.StringToHash("Reload");
            paramShoot = Animator.StringToHash("Shoot");
            paramDeath = Animator.StringToHash("Dead");
        }

        public override void GetMovementCells()
        {
            var psn = Location.Coordinates;

            availableMovePositions[0] = BoardManager.TryGetCell(psn.x + 1, psn.y);
            availableMovePositions[1] = BoardManager.TryGetCell(psn.x - 1, psn.y);
            availableMovePositions[2] = BoardManager.TryGetCell(psn.x, psn.y - 1);
            availableMovePositions[3] = BoardManager.TryGetCell(psn.x, psn.y + 1);

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
                if (cell != null && !cell.IsOccupied)
                {
                    cell.GreenHighlight();
                }
            }
        }

        public override void GetAttackTargets()
        {
            availableTargets = CharacterActions.GetAvailableTargets<EnemyCharacter>(Location, CharacterActions.DirectionType.Diagonal);

            CanAttack = availableTargets.Count > 0 && !HasAttacked;
        }
        
        public override void ShowAttackLocations()
        {
            HideLocations();
            
            attackHighlightedCells = new List<Cell>();
            availableTargets = CharacterActions.GetAvailableTargets<EnemyCharacter>(Location, CharacterActions.DirectionType.Diagonal, attackHighlightedCells);

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
                if (cell != null) cell.DisableHighlight();
            }
            
            foreach (var cell in attackHighlightedCells)
            {
                if (cell != null) cell.DisableHighlight();
            }
        }

        protected override void Move(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            transform.LookAt(cell.Position);

            RegisterMove(cell);

            var pos = transform.position;
            DOVirtual.Vector3(pos, cell.Position, 1, value => transform.position = value);
            DOVirtual.Float(0, 1, 0.5f, value => anim.SetFloat(paramSpeed, value))
                .SetEase(Ease.InOutCubic)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    if (!HasAttacked && Bm.IsCurrentlySelected(this)) ShowAttackLocations();
                    Bm.ShowSelectOverlay(this);
                });
        }

        protected override void Attack(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;

            RegisterAttack();
            
            anim.SetBool(paramAiming, true);
            Vector3 difference = cell.Position - Location.Position;
            var rotY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
            var yAngle = transform.eulerAngles.y;
            if (yAngle - rotY > 180) rotY += 360;
            DOVirtual.Float(yAngle, rotY, 1.25f,
                    value => transform.rotation = Quaternion.Euler(0, value, 0))
                .OnComplete(() =>
                {
                    float delay = Vector2.Distance(cell.Position, Location.Position);
                    attackTarget = cell;
                    Invoke(nameof(ApplyDamage), delay * 0.08f);
                    
                    anim.SetTrigger(paramShoot);
                    CharacterActions.ShootLaser(transform, shotPrefab, shotPosition, cell);
                    Invoke(nameof(Reload), 0.5f);
                    Bm.ShowSelectOverlay(this);
                });
        }
        
        private void ApplyDamage()
        {
            attackTarget.AttackCell(AttackPonints);
            BoardManager.Instance.CheckActions();
        }

        private void Reload()
        {
            anim.SetTrigger(paramReload);
            Invoke(nameof(StopAim), 1.2f);
        }

        private void StopAim()
        {
            anim.SetBool(paramAiming, false);
        }

        public override void Die()
        {
            anim.SetTrigger(paramDeath);
            base.Die();
        }

        public override void GetDamaged()
        {
            GameUIController.Instance.UpdatePlayerDisplay(this);
            // TODO: Add animation for getting damaged
        }
    }
}
