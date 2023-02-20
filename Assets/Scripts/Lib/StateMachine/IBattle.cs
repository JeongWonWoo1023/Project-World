public interface IBattle
{
    void OnDamage(int damage);
    float GetDefence(AttackType type);
    void AttackTarget(float customRange = 0.0f, int CostMana = 0);
    int GetEXP();
    public bool IsDead { get; set; }
}
