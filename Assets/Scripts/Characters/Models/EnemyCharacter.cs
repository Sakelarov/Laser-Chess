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
    }
}
