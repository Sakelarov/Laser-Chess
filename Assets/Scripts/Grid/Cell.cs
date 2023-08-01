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
        [SerializeField] private Material greenMat;
        [SerializeField] private Material redMat;

        private readonly Vector3 psnCorrection = new Vector3(0, 0.12f, 0);
        private MeshRenderer highlight;
        private Character _character;
        private bool _isMovingCell;
        private bool _isAttackingCell;

        public bool IsSelected;
        public bool IsOccupied => _character != null;
        public Vector3 Position { get; private set; }
        public Vector2Int Coordinates { get; private set; }
        [HideInInspector] public UnityEvent<Cell> moveToCell;
        [HideInInspector] public UnityEvent<Cell> attackCell;
        
        private void Awake()
        {
            highlight = transform.GetChild(0).GetComponent<MeshRenderer>();
            
            Position = transform.position + psnCorrection;
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
            if (IsOccupied && !IsSelected && _character is PlayerCharacter ch)
            {
                IsSelected = true;
                BoardManager.Instance.SelectCharacter(ch);
                
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

        public void UnSelectCell()
        {
            IsSelected = false;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }
    }
}
