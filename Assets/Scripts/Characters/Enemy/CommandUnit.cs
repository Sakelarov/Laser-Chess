using Characters.Models;

namespace Characters.Enemy
{
    public class CommandUnit : EnemyCharacter
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
