using UnityEngine;
using UnityEngine.InputSystem;

public class P_RunningState : P_MovingState
{
    #region 필드 & 프로퍼티
    private P_SprintData sprintData;
    private float startTime;
    #endregion

    #region 생성자
    public P_RunningState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        sprintData = groundedData.SprintData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = groundedData.RunData.SpeedCoef;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);

        stateMachine.Current.JumpForce = airborneData.JumpData.RunJumpForce;
        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);
    }

    public override void Process()
    {
        base.Process();
        if (!stateMachine.Current.IsWalk)
        {
            return;
        }
        if (Time.time < startTime + sprintData.RunToWalkTime)
        {
            return;
        }
        StopRunning();

    }
    #endregion

    #region 메인 메소드
    private void StopRunning()
    {
        if (stateMachine.Current.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.Idle);
            return;
        }
        stateMachine.ChangeState(stateMachine.Walk);
    }
    #endregion

    #region 입력 메소드
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.RunToStop);

        base.OnMovementCanceled(context);
    }

    protected override void OnWalkToggle(InputAction.CallbackContext context)
    {
        base.OnWalkToggle(context);

        stateMachine.ChangeState(stateMachine.Walk);
    }
    #endregion
}
