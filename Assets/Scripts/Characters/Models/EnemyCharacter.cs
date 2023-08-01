using Grid;

namespace Characters.Models
{
    public abstract class EnemyCharacter : Character
    {
        public abstract bool TryMove();
        
        public abstract bool TryAttack();
    }
}
