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
        [SerializeField] private Material attackMat;
        [SerializeField] private Material movementMat;
       
        private readonly Vector3 psnCorrection = new Vector3(0, 0.12f, 0);
        private readonly Color redTransp = new Color(1, 0, 0, 0);
        private readonly Color red40 = new Color(1, 0, 0, 0.40f);
        private readonly Color greenTransp = new Color(0, 1, 0, 0);
        private readonly Color green = new Color(0, 1, 0, 1f);
        private MeshRenderer highlight;
        private bool _isMovingCell;
        private bool _isAttackingCell;

        public bool IsSelected { get; private set; }
        public bool IsOccupied => Character != null;
        public Character Character { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector2Int Coordinates { get; private set; }
        public static float BlinkDuration => 0.65f;

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
            
            Material mat = Instantiate(movementMat);
            
            DOVirtual.Color(greenTransp, green, BlinkDuration, value => mat.color = value)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(highlight.gameObject, LinkBehaviour.KillOnDisable);
            
            highlight.material = mat;
            highlight.gameObject.SetActive(true);
        }

        public void RedBlink(bool isTarget, int loops)
        {
            _isAttackingCell = isTarget;
            
            Material mat = Instantiate(attackMat);
            
            DOVirtual.Color(redTransp, red40, BlinkDuration, value => mat.color = value)
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
            if (Character.HealthPoints < damage)
            {
                BoardManager.Instance.DisplayDamage(this, Character.HealthPoints);
                Character.HealthPoints = 0;
            }
            else
            {
                BoardManager.Instance.DisplayDamage(this, damage);
                Character.HealthPoints -= damage;
            } 
            
            if(Character.HealthPoints == 0) Character.Die();
            else Character.GetDamaged();
        }

        public void ResetCell()
        {
            Character = null;
            IsSelected = false;
            DisableHighlight();
        }
    }
}
