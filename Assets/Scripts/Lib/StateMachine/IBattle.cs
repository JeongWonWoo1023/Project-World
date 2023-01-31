public interface IBattle
{
    void OnDamage(int damage);
    float GetDefence(AttackType type);
    void AttackTarget();
    int GetEXP();
    public bool IsDead { get; set; }
}
