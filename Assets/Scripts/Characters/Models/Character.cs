using Grid;
using UnityEngine;

namespace Characters.Models
{
    public abstract class Character : MonoBehaviour
    {
        protected BoardManager Bm;
        protected Cell Location;
        protected int HealthPoints;
        protected int AttackPonints;
        
        public bool HasMoved;
        public bool HasAttacked;
        

        public virtual void Setup(Cell cell)
        {
            Bm = BoardManager.Instance;
            Location = cell;
            transform.position = cell.Position;
        }
        
        
        public virtual void ResetTurn()
        {
            HasMoved = false;
            HasAttacked = false;
        }
    }
}
