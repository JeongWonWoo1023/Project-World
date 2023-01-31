using UnityEngine.InputSystem;

public class P_WalkingState : P_MovingState
{
    #region 필드 & 프로퍼티
    private P_WalkData walkData;
    #endregion

    #region 생성자
    public P_WalkingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        walkData = groundedData.WalkData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = groundedData.WalkData.SpeedCoef;

        stateMachine.Current.BackwardCameraRecenteringData = walkData.BackwardCameraRecenteringData;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);

        stateMachine.Current.JumpForce = airborneData.JumpData.WalkJumpForce;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
    }
    #endregion

    #region 입력 메소드
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.WalkToStop);

        base.OnMovementCanceled(context);
    }

    protected override void OnWalkToggle(InputAction.CallbackContext context)
    {
        base.OnWalkToggle(context);

        stateMachine.ChangeState(stateMachine.Run);
    }
    #endregion
}
