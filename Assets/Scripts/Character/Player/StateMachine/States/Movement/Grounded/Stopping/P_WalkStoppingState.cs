public class P_WalkStoppingState : P_StoppingState
{
    #region 생성자
    public P_WalkStoppingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        stateMachine.Current.MovementDecelerationForce = groundedData.StopData.WalkDecelerationForce;

        stateMachine.Current.JumpForce = airborneData.JumpData.WalkJumpForce;
    }
    #endregion
}
