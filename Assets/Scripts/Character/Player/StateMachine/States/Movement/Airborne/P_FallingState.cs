using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_FallingState : P_AirborneState
{
    #region 필드 & 프로퍼티
    private P_FallData fallData;
    private P_JumpData jumpData;

    private Vector3 playerPositionOnEnter;
    #endregion

    #region 생성자
    public P_FallingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        fallData = airborneData.FallData;
        jumpData = airborneData.JumpData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = 0.0f;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.FallParameterHash);

        playerPositionOnEnter = stateMachine.Player.transform.position;

        ResetVerticalVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        if(stateMachine.Player.Rigid.useGravity)
        {
            stateMachine.Player.Rigid.useGravity = false;
        }
        StopAnimation(stateMachine.Player.AnimationData.FallParameterHash);
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        LimitVerticalVelocity();
    }
    #endregion

    #region 메인 메소드
    private void LimitVerticalVelocity()
    {
        Vector3 playerVerticalvelocity = movementUtil.GetVerticalVelocity(stateMachine.Player.Rigid.velocity);
        if (playerVerticalvelocity.y >= -fallData.FallSpeedLimit)
        {
            return;
        }

        Vector3 limitedvelocity = new Vector3(0.0f, -fallData.FallSpeedLimit - playerVerticalvelocity.y, 0.0f);

        stateMachine.Player.Rigid.AddForce(limitedvelocity, ForceMode.VelocityChange);
    }
    #endregion

    #region 재사용 메소드
    protected override void ResetSprintState() { }

    protected override void OnContectGround(Collider collider)
    {
        float fallDistance = playerPositionOnEnter.y - stateMachine.Player.transform.position.y;

        if (fallDistance < fallData.MinimumDistanceToBeConsideredHardFall)
        {
            // 이동 입력이 있을 경우
            if (stateMachine.Current.MovementInput != Vector2.zero)
            {
                if (stateMachine.Current.JumpForce == jumpData.WalkJumpForce)
                {
                    stateMachine.ChangeState(stateMachine.Walk);
                    return;
                }

                if (stateMachine.Current.JumpForce == jumpData.RunJumpForce)
                {
                    stateMachine.ChangeState(stateMachine.Run);
                    return;
                }

                if (stateMachine.Current.JumpForce == jumpData.SprintJumpForce)
                {
                    stateMachine.ChangeState(stateMachine.Sprint);
                    return;
                }
            }
            stateMachine.ChangeState(stateMachine.LightLanding);

            return;
        }

        if (stateMachine.Current.IsWalk && !stateMachine.Current.IsSprint || stateMachine.Current.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.HardLanding);

            return;
        }

        stateMachine.ChangeState(stateMachine.Rolling);
    }
    #endregion
}
