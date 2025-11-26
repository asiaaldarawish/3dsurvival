public interface IAttackable
{
    bool CanAttack(PlayerBootstrap player);
    void Attack(PlayerBootstrap player);
}