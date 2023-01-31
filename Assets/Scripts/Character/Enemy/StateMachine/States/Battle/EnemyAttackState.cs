public class EnemyAttackState : EnemyBattleState
{
    public EnemyAttackState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        // 모션 시작
        base.Enter();
        agent.updatePosition = false;
        SetAnimationIntValue("animation", (int)EnemyAnimationType.Attack);
    }

    public override void Exit()
    {
        // 모션 종료
        base.Exit();
        agent.updatePosition = true;
    }

    public override void OnAnimationEnter()
    {
        // 공격 판정
        base.OnAnimationEnter();
        stateMachine.Enemy.SkiilDamageCoef = 1.0f;
        stateMachine.Enemy.AttackTarget();
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.ChangeState(stateMachine.TargetChasingMove);
    }
    #endregion
}
