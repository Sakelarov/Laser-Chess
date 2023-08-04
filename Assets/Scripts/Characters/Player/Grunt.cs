using System.Collections;
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
        
        private Cell _topCell, _bottomCell, _leftCell, _rightCell; // movement cells

        private Animator anim;
        private int paramSpeed, paramAiming, paramReload, paramShoot, paramDeath;

        private Cell attackTarget;
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            GetMovementCells();

            anim = GetComponentInChildren<Animator>();
            paramSpeed = Animator.StringToHash("Speed");
            paramAiming = Animator.StringToHash("Aiming");
            paramReload = Animator.StringToHash("Reload");
            paramShoot = Animator.StringToHash("Shoot");
            paramDeath = Animator.StringToHash("Dead");
        }

        private void GetMovementCells()
        {
            var psn = Location.Coordinates;
           
            _topCell = BoardManager.TryGetCell(psn.x + 1, psn.y);
            _bottomCell = BoardManager.TryGetCell(psn.x - 1, psn.y);
            _leftCell = BoardManager.TryGetCell(psn.x, psn.y - 1);
            _rightCell = BoardManager.TryGetCell(psn.x, psn.y + 1);
        }

        protected override void ShowMoveLocations()
        {
            if (_topCell != null && !_topCell.IsOccupied)
                _topCell.GreenHighlight();
            if (_bottomCell != null && !_bottomCell.IsOccupied)
                _bottomCell.GreenHighlight();
            if (_leftCell != null && !_leftCell.IsOccupied)
                _leftCell.GreenHighlight();
            if (_rightCell != null && !_rightCell.IsOccupied)
                _rightCell.GreenHighlight();
        }

        protected override void ShowAttackLocations()
        {
            attackHighlightedCells = new List<Cell>();
            availableTargets = CharacterActions.GetAvailableTargets<EnemyCharacter>(Location, CharacterActions.DirectionType.Diagonal, attackHighlightedCells);

            if (availableTargets.Count > 0)
            {
                AnimateAttackLocations();
            }
            else HasAttacked = true;
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
            if (_topCell != null) 
                _topCell.DisableHighlight();
            if (_bottomCell != null)
                _bottomCell.DisableHighlight();
            if (_leftCell != null)
                _leftCell.DisableHighlight();
            if (_rightCell != null)
                _rightCell.DisableHighlight();

            foreach (var cell in attackHighlightedCells)
            {
                cell.DisableHighlight();
            }
        }

        protected override void Move(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            transform.LookAt(cell.Position);
            
            HasMoved = true;
            HideLocations();
            Bm.HideSelectOverlay();
            
            Location.SetCharacter(null);
            Location = cell;
            cell.SetCharacter(this);
            GetMovementCells();
            
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
            
            HasAttacked = true;
            HideLocations();
            Bm.HideSelectOverlay();
            
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
                    Invoke(nameof(ApplyDamage), delay * 0.04f);
                    
                    anim.SetTrigger(paramShoot);
                    CharacterActions.ShootLaser(transform, shotPrefab, shotPosition, cell);
                    Invoke(nameof(Reload), 0.5f);
                    Bm.ShowSelectOverlay(this);
                });
        }
        
        private void ApplyDamage()
        {
            attackTarget.AttackCell(AttackPonints);
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
            
        }

        public override void GetDamaged()
        {
            
        }
    }
}
