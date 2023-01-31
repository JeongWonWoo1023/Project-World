public class P_MovingState : PlayerGroundedState
{
    #region 생성자
    public P_MovingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.MovingParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.MovingParameterHash);
    }
    #endregion
}
