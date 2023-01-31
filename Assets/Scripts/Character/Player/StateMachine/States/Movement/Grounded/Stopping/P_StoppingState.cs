using UnityEngine.InputSystem;

public class P_StoppingState : PlayerGroundedState
{

    #region 생성자
    public P_StoppingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.StoppingParameterHash);

        stateMachine.Current.MovementSpeedCoef = 0.0f;

        SetBaseCameraRecenteringData();
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.StoppingParameterHash);
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        RotateToTargetRotation();

        if (!movementUtil.IsMoving(stateMachine.Player.Rigid.velocity))
        {
            return;
        }
        DecelerateHorizontally();
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

        stateMachine.Player.Input.InGameActions.Movement.started += OnMovementStarted;
    }
    #endregion

    #region 입력 메소드
    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        OnMove();
    }
    #endregion
}
