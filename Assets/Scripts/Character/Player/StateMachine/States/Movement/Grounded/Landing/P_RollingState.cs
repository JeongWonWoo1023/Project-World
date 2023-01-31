using UnityEngine;
using UnityEngine.InputSystem;

public class P_RollingState : P_LandingState
{
    #region 필드 & 프로퍼티
    private P_RollingData rollData;
    #endregion

    #region 생성자
    public P_RollingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        rollData = groundedData.RollingData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = rollData.SpeedCoef;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);

        stateMachine.Current.IsSprint = false;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        if (stateMachine.Current.MovementInput != Vector2.zero)
        {
            return;
        }

        RotateToTargetRotation();
    }

    public override void OnAnimationTransition()
    {
        if (stateMachine.Current.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.RunToStop);
            return;
        }

        OnMove();
    }
    #endregion

    #region 입력 메소드
    protected override void OnJumpStared(InputAction.CallbackContext context) { }
    #endregion
}
