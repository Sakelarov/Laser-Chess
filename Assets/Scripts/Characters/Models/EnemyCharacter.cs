using Grid;
using UnityEngine;

namespace Characters.Models
{
    public abstract class EnemyCharacter : Character
    {
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        public abstract bool TryMove(ref float delay);
        
        public abstract bool TryAttack(ref float delay);
        
        protected virtual void RegisterMove(Cell cell)
        {
            transform.LookAt(cell.Position);
            HasMoved = true;
            portrait.DisableMoveIndicator();
            Location.SetCharacter(null);
            Location = cell;
            portrait.UpdatePosition();
            cell.SetCharacter(this);
            GameUIController.Instance.UpdateEnemyDisplay(this);
        }
        
        public override void Die()
        {
            base.Die();
            GameUIController.Instance.DeactiveEnemy(this);
        }
    }
}
