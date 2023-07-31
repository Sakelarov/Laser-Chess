using Grid;
using DG.Tweening;
using UnityEngine;

namespace Characters
{
    public class Grunt : PlayerCharacter
    {
        private Cell _topCell;
        private Cell _bottomCell;
        private Cell _leftCell;
        private Cell _rightCell;

        private Animator anim;
        private int paramSpeed;
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            GetNewCells();

            anim = GetComponentInChildren<Animator>();
            paramSpeed = Animator.StringToHash("Speed");
        }

        private void GetNewCells()
        {
            var grid = Bm.board;
            var psn = Location.Coordinates;
            _topCell = psn.x + 1 < 8 ? grid[psn.x + 1, psn.y] : null;
            _bottomCell = psn.x - 1 >= 0 ? grid[psn.x - 1, psn.y] : null;
            _leftCell = psn.y - 1 >= 0 ? grid[psn.x, psn.y - 1] : null;
            _rightCell = psn.y + 1 < 8 ? grid[psn.x, psn.y + 1] : null;
        }

        protected override void ShowMoveLocations()
        {
            if (!_topCell.IsOccupied) _topCell.GreenHighlight();
            if (!_bottomCell.IsOccupied) _bottomCell.GreenHighlight();
            if (!_leftCell.IsOccupied) _leftCell.GreenHighlight();
            if (!_rightCell.IsOccupied) _rightCell.GreenHighlight();
        }

        protected override void HideMoveLocations()
        {
            _topCell.DisableHighlight();
            _bottomCell.DisableHighlight();
            _leftCell.DisableHighlight();
            _rightCell.DisableHighlight();
        }

        protected override void Move(Cell cell)
        {
            if (cell == _topCell) transform.eulerAngles = Vector3.zero;
            else if (cell == _bottomCell) transform.eulerAngles = new Vector3(0, 180, 0);
            else if (cell == _leftCell) transform.eulerAngles = new Vector3(0, -90, 0);
            else if (cell == _rightCell) transform.eulerAngles = new Vector3(0, 90, 0);
            
            HasMoved = true;
            HideMoveLocations();
            
            Location = cell;
            GetNewCells();
            
            var pos = transform.position;
            DOVirtual.Vector3(pos, cell.Position, 1, value => transform.position = value);
            DOVirtual.Float(0, 1, 0.5f, value => anim.SetFloat(paramSpeed, value))
                .SetEase(Ease.InOutCubic)
                .SetLoops(2, LoopType.Yoyo);
        }
    }
}
