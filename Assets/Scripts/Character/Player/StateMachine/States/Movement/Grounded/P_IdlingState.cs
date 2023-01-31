using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_IdlingState : PlayerGroundedState
{
    #region 필드 & 프로퍼티
    private P_IdleData idleData;
    #endregion

    #region 생성자
    public P_IdlingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        idleData = groundedData.IdleData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = 0.0f;

        stateMachine.Current.BackwardCameraRecenteringData = idleData.BackwardCameraRecenteringData;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);

        stateMachine.Current.JumpForce = airborneData.JumpData.StandJumpForce;
        ResetVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Process()
    {
        base.Process();

        if (stateMachine.Current.MovementInput == Vector2.zero) return;

        OnMove();
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
    #endregion
}
