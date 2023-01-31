using UnityEngine;

public class EnemyHitState : EnemyBattleState
{
    public EnemyHitState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;
        SetAnimationIntValue("animation", (int)EnemyAnimationType.Damage);
    }

    public override void Exit()
    {
        base.Exit();
        agent.updatePosition = true;
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.ChangeState(stateMachine.TargetChasingMove);
    }
}
