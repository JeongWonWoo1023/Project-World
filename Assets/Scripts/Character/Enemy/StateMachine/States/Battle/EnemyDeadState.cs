using UnityEngine;

public class EnemyDeadState : EnemyBattleState
{
    public EnemyDeadState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;
        SetAnimationIntValue("animation", (int)EnemyAnimationType.Die);
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.Enemy.gameObject.SetActive(false);
        ObjectPool.Instance.PushPool(stateMachine.Enemy.gameObject);
    }
}
