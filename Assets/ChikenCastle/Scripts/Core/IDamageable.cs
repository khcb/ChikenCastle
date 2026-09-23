public interface IDamageable
{
    Team Team { get; }
    void TakeDamage(float amount);
    void Die();
}