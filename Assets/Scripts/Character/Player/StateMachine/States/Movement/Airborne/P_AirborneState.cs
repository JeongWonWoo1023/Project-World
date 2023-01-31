using UnityEngine;

public class P_AirborneState : PlayerMovementState
{

    #region 생성자
    public P_AirborneState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Rigid.useGravity = true;
        StartAnimation(stateMachine.Player.AnimationData.AirborneParameterHash);

        ResetSprintState();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.AirborneParameterHash);
    }
    #endregion

    #region 재사용 메소드
    protected override void OnContectGround(Collider collider)
    {
        stateMachine.ChangeState(stateMachine.LightLanding);
    }

    protected override void AddInputAction()
    {
        base.AddInputAction();
        stateMachine.Player.Input.InGameActions.Movement.performed -= OnMovementPerformed;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
        stateMachine.Player.Input.InGameActions.Movement.performed += OnMovementPerformed;
    }
    #endregion

    #region 재사용 가상 메소드
    protected virtual void ResetSprintState()
    {
        stateMachine.Current.IsSprint = false;
    }
    #endregion
}
