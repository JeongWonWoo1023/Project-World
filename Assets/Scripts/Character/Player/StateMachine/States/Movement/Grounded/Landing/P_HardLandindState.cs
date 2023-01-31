using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_HardLandindState : P_LandingState
{
    #region 생성자
    public P_HardLandindState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = 0.0f;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.HardLandParameterHash);

        stateMachine.Player.Input.InGameActions.Movement.Disable();

        ResetVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.HardLandParameterHash);

        stateMachine.Player.Input.InGameActions.Movement.Enable();
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        if (!movementUtil.IsMoving(stateMachine.Player.Rigid.velocity))
        {
            return;
        }

        ResetVelocity();
    }

    public override void OnAnimationExit()
    {
        stateMachine.Player.Input.InGameActions.Movement.Enable();
    }

    public override void OnAnimationTransition()
    {
        stateMachine.ChangeState(stateMachine.Idle);
    }
    #endregion

    #region 재사용 메소드
    protected override void AddInputAction()
    {
        base.AddInputAction();

        stateMachine.Player.Input.InGameActions.Movement.started += OnMovementStarted;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();

        stateMachine.Player.Input.InGameActions.Movement.started -= OnMovementStarted;
    }

    protected override void OnMove()
    {
        if (stateMachine.Current.IsWalk)
        {
            return;
        }

        stateMachine.ChangeState(stateMachine.Run);
    }
    #endregion

    #region 입력 메소드
    protected override void OnJumpStared(InputAction.CallbackContext context) { }

    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        OnMove();
    }
    #endregion
}
