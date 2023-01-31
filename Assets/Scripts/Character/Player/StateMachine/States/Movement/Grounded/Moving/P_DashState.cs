using UnityEngine;
using UnityEngine.InputSystem;

public class P_DashState : PlayerGroundedState
{
    #region 필드 & 프로퍼티
    private P_DashData dashData;
    private float startTime;
    private int consecutiveDashesUsed;
    private bool keepRotating;
    #endregion

    #region 생성자
    public P_DashState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        dashData = groundedData.DashData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = groundedData.DashData.SpeedCoef;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);

        stateMachine.Current.RotationData = dashData.RotationData;

        Dash();

        keepRotating = stateMachine.Current.MovementInput != Vector2.zero;

        UpdateConsecutiveDashes();

        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.DashParameterHash);
        SetRotationData();
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        if (!keepRotating)
        {
            return;
        }

        RotateToTargetRotation();
    }

    public override void OnAnimationTransition()
    {
        if (stateMachine.Current.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.SprintToStop);
            return;
        }
        stateMachine.ChangeState(stateMachine.Sprint);
    }
    #endregion

    #region 메인 메소드
    // 대쉬
    private void Dash()
    {
        Vector3 dashDirection = stateMachine.Player.transform.forward;
        dashDirection.y = 0.0f;

        UpdateTargetRotation(dashDirection, false);

        if (stateMachine.Current.MovementInput != Vector2.zero)
        {
            UpdateTargetRotation(GetInputDirection());

            dashDirection = movementUtil.GetTargetRotation(stateMachine.Current.CurrentTargetRotation.y);
        }

        stateMachine.Player.Rigid.velocity = dashDirection * GetMovementSpeed(false);
    }

    // 대쉬 연속사용 갱신
    private void UpdateConsecutiveDashes()
    {
        if (!IsCondecutive())
        {
            consecutiveDashesUsed = 0;
        }
        ++consecutiveDashesUsed;

        if (consecutiveDashesUsed == dashData.UseLimitCount)
        {
            consecutiveDashesUsed = 0;
            stateMachine.Player.Input.CallDisableAction(stateMachine.Player.Input.InGameActions.Dash, dashData.CollTime);
        }
    }

    // 연속으로 사용하지 않은 경우
    private bool IsCondecutive()
    {
        return Time.time < startTime + dashData.ConsideredTime;
    }
    #endregion

    #region 재사용 메소드
    // 입력 바인딩 등록 재정의
    protected override void AddInputAction()
    {
        base.AddInputAction();

        stateMachine.Player.Input.InGameActions.Movement.performed += OnMovementPerformed;
    }

    // 입력 바인딩 해제 재정의
    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();

        stateMachine.Player.Input.InGameActions.Movement.performed -= OnMovementPerformed;
    }
    #endregion

    #region 입력 메소드
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        keepRotating = true;
    }

    protected override void OnDashStarted(InputAction.CallbackContext context) { }
    #endregion
}
