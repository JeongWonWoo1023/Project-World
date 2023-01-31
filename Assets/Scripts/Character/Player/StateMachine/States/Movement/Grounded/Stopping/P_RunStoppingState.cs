public class P_RunStoppingState : P_StoppingState
{
    #region 생성자
    public P_RunStoppingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.MediumStopParameterHash);
        stateMachine.Current.MovementDecelerationForce = groundedData.StopData.RunDecelerationForce;

        stateMachine.Current.JumpForce = airborneData.JumpData.RunJumpForce;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.MediumStopParameterHash);
    }
    #endregion
}
