using Characters.Models;

namespace Characters.Enemy
{
    public class Dreadnought : EnemyCharacter
    {
        public override bool TryMove()
        {
            return false;
        }

        public override bool TryAttack()
        {
            return false;
        }
    }
}
