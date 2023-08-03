using Characters;
using Characters.Models;
using DG.Tweening;
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
        private readonly Color from = new Color(1, 0, 0, 0);
        private readonly Color to = new Color(1, 0, 0, 0.25f);
        private MeshRenderer highlight;
        private bool _isMovingCell;
        private bool _isAttackingCell;

        public bool IsSelected;
        public bool IsOccupied => Character != null;
        public Character Character { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector2Int Coordinates { get; private set; }
        
        [HideInInspector] public UnityEvent<Cell> moveToCell;
        [HideInInspector] public UnityEvent<Cell> attackCell;

        // Used by Pathfinder
        [HideInInspector] public Cell parent;
        [HideInInspector] public int gCost;
        [HideInInspector] public int hCost;
        public int FCost => gCost + hCost;
        
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

        public void RedBlink(bool isTarget, int loops)
        {
            _isAttackingCell = isTarget;
            
            Material mat = Instantiate(redMat);
            
            DOVirtual.Color(from, to, 0.65f, value => mat.color = value)
                .SetEase(Ease.InOutSine)
                .SetLoops(loops, LoopType.Yoyo)
                .SetLink(highlight.gameObject, LinkBehaviour.KillOnDisable);
            
            highlight.material = mat;
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
            if (IsOccupied && !IsSelected && Character is PlayerCharacter ch)
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
            Character = character;
        }

        public void AttackCell(int damage)
        {
            
        }
    }
}
