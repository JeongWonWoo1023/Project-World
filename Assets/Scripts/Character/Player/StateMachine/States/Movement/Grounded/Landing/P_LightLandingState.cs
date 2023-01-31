using UnityEngine;

public class P_LightLandingState : P_LandingState
{
    #region 생성자
    public P_LightLandingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = 0.0f;

        base.Enter();

        stateMachine.Current.JumpForce = airborneData.JumpData.StandJumpForce;

        ResetVelocity();
    }

    public override void Process()
    {
        base.Process();

        if (stateMachine.Current.MovementInput == Vector2.zero)
        {
            return;
        }

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

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();

        stateMachine.ChangeState(stateMachine.Idle);
    }
    #endregion
}
