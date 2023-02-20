using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBattleState
{
    public PlayerDeadState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Anim.SetBool("IsDead",stateMachine.Player.IsDead);
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();

    }
}
