using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHitState : PlayerBattleState
{
    public PlayerHitState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Anim.SetTrigger("TrigHit");
        AddInputAction();
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.ChangeState(stateMachine.Idle);
        RemoveInputAction();
    }

    protected override void AddInputAction()
    {
        base.AddInputAction();
        stateMachine.Player.Input.InGameActions.NormalAttack.performed += OnNormalAtackAction;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
        stateMachine.Player.Input.InGameActions.NormalAttack.performed -= OnNormalAtackAction;
    }

    protected override void OnNormalAtackAction(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.NormalAttack);
    }
}
