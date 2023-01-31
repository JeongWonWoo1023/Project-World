using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_SprintingState : P_MovingState
{
    #region 필드 & 프로퍼티
    private P_SprintData sprintData;

    private float startTime;
    private bool keepSprinting;
    private bool resetSprintState;
    #endregion

    #region 생성자
    public P_SprintingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        sprintData = groundedData.SprintData; 
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = sprintData.SpeedCoef;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.SprintParameterHash);

        stateMachine.Current.JumpForce = airborneData.JumpData.SprintJumpForce;
        resetSprintState = true;
        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.SprintParameterHash);

        if (resetSprintState)
        {
            keepSprinting = false;
            stateMachine.Current.IsSprint = false;
        }
    }

    public override void Process()
    {
        base.Process();
        if (keepSprinting)
        {
            return;
        }
        if (Time.time < startTime + sprintData.SprintToRunTime)
        {
            return;
        }

        StopSprinting();
    }
    #endregion

    #region 메인 메소드
    private void StopSprinting()
    {
        if (stateMachine.Current.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.Idle);
            return;
        }
        stateMachine.ChangeState(stateMachine.Run);
    }
    #endregion

    #region 재사용 메소드
    protected override void AddInputAction()
    {
        base.AddInputAction();
        stateMachine.Player.Input.InGameActions.Sprint.performed += OnSprintPerformed;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
    }

    protected override void OnFall()
    {
        resetSprintState = false;

        base.OnFall();
    }
    #endregion

    #region 입력 메소드
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.SprintToStop);

        base.OnMovementCanceled(context);
    }

    protected override void OnJumpStared(InputAction.CallbackContext context)
    {
        resetSprintState = false;

        base.OnJumpStared(context);
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        keepSprinting = true;

        stateMachine.Current.IsSprint = true;
    }
    #endregion
}
