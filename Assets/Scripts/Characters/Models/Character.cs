using System.Collections.Generic;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Models
{
    public abstract class Character : MonoBehaviour
    {
        protected BoardManager Bm;
        
        public int HealthPoints;
        public int AttackPonints;
        public Cell Location { get; protected set; }
        public bool HasMoved { get; protected set; }
        public bool HasAttacked { get; protected set; }
        

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

        public abstract void Die();

        public abstract void GetDamaged();
    }
}
