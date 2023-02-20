using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyStateCore
{
    public EnemyIdleState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;
        SetAnimationIntValue("animation", (int)EnemyAnimationType.Idle);
    }

    public override void Exit()
    {
        base.Exit();
        agent.updatePosition = true;
    }
}
