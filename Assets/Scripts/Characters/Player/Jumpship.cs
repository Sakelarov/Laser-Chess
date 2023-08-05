using System.Collections.Generic;
using Characters.Models;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Player
{
    public class Jumpship : PlayerCharacter
    {
        [SerializeField] private ParticleSystem sparks;
        [SerializeField] private GameObject attackEffect;
        private List<Cell> attackTargets = new List<Cell>();
        private Cell _topCell, _bottomCell, _leftCell, _rightCell;
        
        private Cell[] availableMovePositions = new Cell[8];

        private Animator anim;
        private int paramJump, paramDie, paramSpeed;

        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            GetMovementCells();

            anim = GetComponentInChildren<Animator>();
            paramJump = Animator.StringToHash("jump");
            paramDie = Animator.StringToHash("die");
            paramSpeed = Animator.StringToHash("speed");
        }
    
        protected override void GetMovementCells()
        {
            var psn = Location.Coordinates;
            
            availableMovePositions[0] = BoardManager.TryGetCell(psn.x + 2, psn.y + 1);
            availableMovePositions[1] = BoardManager.TryGetCell(psn.x + 2, psn.y - 1);
            availableMovePositions[2] = BoardManager.TryGetCell(psn.x + 1, psn.y + 2);
            availableMovePositions[3] = BoardManager.TryGetCell(psn.x + 1, psn.y - 2);
            availableMovePositions[4] = BoardManager.TryGetCell(psn.x - 1, psn.y + 2);
            availableMovePositions[5] = BoardManager.TryGetCell(psn.x - 1, psn.y - 2);
            availableMovePositions[6] = BoardManager.TryGetCell(psn.x - 2, psn.y + 1);
            availableMovePositions[7] = BoardManager.TryGetCell(psn.x - 2, psn.y - 1);
        }
    
        public override void ShowMoveLocations()
        {
            HideLocations();
            
            foreach (var cell in availableMovePositions)
            {
                if(cell != null && !cell.IsOccupied) cell.GreenHighlight();
            }
        }
        
        private void GetAttackCells()
        {
            var psn = Location.Coordinates;
           
            _topCell = BoardManager.TryGetCell(psn.x + 1, psn.y);
            _bottomCell = BoardManager.TryGetCell(psn.x - 1, psn.y);
            _leftCell = BoardManager.TryGetCell(psn.x, psn.y - 1);
            _rightCell = BoardManager.TryGetCell(psn.x, psn.y + 1);
        }
        
        public override void ShowAttackLocations()
        {
            HideLocations();
            GetAttackCells();

            attackTargets = new List<Cell>();

            if (_topCell != null && _topCell.IsOccupied && _topCell.Character is EnemyCharacter)
            {
                _topCell.RedBlink(true, -1);
                attackTargets.Add(_topCell);
            }
            if (_bottomCell != null && _bottomCell.IsOccupied && _bottomCell.Character is EnemyCharacter)
            {
                _bottomCell.RedBlink(true, -1);
                attackTargets.Add(_bottomCell);
            }
            if (_leftCell != null && _leftCell.IsOccupied && _leftCell.Character is EnemyCharacter)
            {
                _leftCell.RedBlink(true, -1);
                attackTargets.Add(_leftCell);
            }
            if (_rightCell != null && _rightCell.IsOccupied && _rightCell.Character is EnemyCharacter)
            {
                _rightCell.RedBlink(true, -1);
                attackTargets.Add(_rightCell);
            }

            if (attackTargets.Count == 0) HasAttacked = true;
        }

        protected override void HideLocations()
        {
            foreach (var cell in availableMovePositions)
            {
                if(cell != null) cell.DisableHighlight();
            }

            foreach (var target in attackTargets)
            {
                target.DisableHighlight();
            }
        }

        protected override void Move(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            transform.LookAt(cell.Position);

            RegisterMove(cell);

            anim.SetFloat(paramSpeed, 1);
            anim.SetTrigger(paramJump);
            var pos = transform.position;
            var target = cell.Position;
            var midPoint = (pos + target + new Vector3(0, 2, 0)) / 2;
            DOVirtual.Vector3(pos, midPoint, 0.5f, value => transform.position = value)
                .SetEase(Ease.OutSine)
                .SetDelay(0.5f)
                .OnComplete(() =>
                {
                    DOVirtual.Vector3(midPoint, target, 0.5f, value => transform.position = value)
                        .SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            if (!HasAttacked && Bm.IsCurrentlySelected(this)) ShowAttackLocations();
                            Bm.ShowSelectOverlay(this);
                        });
                });
        }
        
        protected override void Attack(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            RegisterAttack();
            
            anim.SetFloat(paramSpeed, 2);
            anim.SetTrigger(paramJump);
            var pos = transform.position;
            var endPos = pos + new Vector3(0, 0.25f, 0);
            DOVirtual.Vector3(pos, endPos, 0.3f, value => transform.position = value)
                .SetEase(Ease.OutSine)
                .SetDelay(0.3f)
                .OnComplete(() =>
                {
                    DOVirtual.Vector3(endPos, pos, 0.3f, value => transform.position = value)
                        .SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            foreach (var target in attackTargets)
                            {
                                var attack = Instantiate(attackEffect);
                                attack.transform.position = transform.position;
                                attack.transform.eulerAngles = target == _topCell ? new Vector3(0, 0, 0) :
                                    target == _bottomCell ? new Vector3(0, 180, 0) :
                                    target == _rightCell ? new Vector3(0, 90, 0) :
                                    target == _leftCell ? new Vector3(0, 270, 0) : new Vector3(0, 0, 0);
                            }
                            Invoke(nameof(ApplyDamage), 0.45f);
                            Bm.ShowSelectOverlay(this);
                        });
                });
        }

        private void ApplyDamage()
        {
            foreach (var target in attackTargets)
            {
                target.AttackCell(AttackPonints);
            }
        }
        
        public override void Die()
        {
            anim.SetTrigger(paramDie);
            sparks.Play();
            base.Die();
        }

        public override void GetDamaged()
        {
            GameUIController.Instance.UpdatePlayerInfo(this);
            sparks.Play();
        }
    }
}
