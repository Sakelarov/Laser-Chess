using System.Collections.Generic;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Models
{
    public abstract class Character : MonoBehaviour
    {
        protected BoardManager Bm;

        public UnitPortrait portrait;
        public int HealthPoints;
        public int AttackPonints;
        public Cell Location { get; protected set; }
        public bool HasMoved { get; set; }
        public bool HasAttacked { get; set; }
        public bool IsDead => HealthPoints <= 0;

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
            portrait.ResetTurn();
        }

        public virtual void Die()
        {
            Location.SetCharacter(null);
            Destroy(gameObject, 4f);
            portrait.Hide();
        }

        public abstract void GetDamaged();
    }
}
