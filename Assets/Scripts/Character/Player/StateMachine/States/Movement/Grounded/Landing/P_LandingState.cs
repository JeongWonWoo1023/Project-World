public class P_LandingState : PlayerGroundedState
{
    #region 생성자
    public P_LandingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        if(stateMachine.Player.Rigid.useGravity)
        {
            stateMachine.Player.Rigid.useGravity = false;
        }
        StartAnimation(stateMachine.Player.AnimationData.LandingParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.LandingParameterHash);
    }
    #endregion
}
