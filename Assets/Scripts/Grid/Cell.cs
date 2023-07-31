using Characters;
using Characters.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Grid
{
    [System.Serializable]
    public class Cell : MonoBehaviour
    {
        [SerializeField] private MeshRenderer highlight;
        [SerializeField] private Material greenMat;
        [SerializeField] private Material redMat;
        
        private Character _character;
        private bool _isMovingCell;
        private bool _isAttackingCell;

        public bool IsSelected;
        public bool IsOccupied => _character != null;
        public Vector3 Position { get; private set; }
        public Vector2Int Coordinates { get; private set; }
        public UnityEvent<Cell> moveToCell;
        public UnityEvent<Cell> attackCell;
        
        private void Awake()
        {
            Position = transform.position;
            Coordinates = new Vector2Int(Mathf.RoundToInt(Position.z), Mathf.RoundToInt(Position.x));
        }

        public void GreenHighlight()
        {
            _isMovingCell = true;
            highlight.material = greenMat;
            highlight.gameObject.SetActive(true);
        }

        public void RedHighlight()
        {
            _isAttackingCell = true;
            highlight.material = redMat;
            highlight.gameObject.SetActive(true);
        }

        public void DisableHighlight()
        {
            _isAttackingCell = false;
            _isMovingCell = false;
            highlight.gameObject.SetActive(false);
        }

        public CellState SelectCell()
        {
            if (IsOccupied && !IsSelected)
            {
                IsSelected = true;
                if (_character is PlayerCharacter ch) ch.SelectCharacter();

                return CellState.Select;
            }

            if (_isMovingCell)
            {
                moveToCell.Invoke(this);
                return CellState.Move;
            }

            if (_isAttackingCell)
            {
                attackCell.Invoke(this);
                return CellState.Attack;
            }

            return CellState.None;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }
    }
}
