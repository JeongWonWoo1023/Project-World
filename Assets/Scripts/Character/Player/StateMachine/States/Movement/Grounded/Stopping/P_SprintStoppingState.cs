public class P_SprintStoppingState : P_StoppingState
{
    #region 생성자
    public P_SprintStoppingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.HardStopParameterHash);

        stateMachine.Current.MovementDecelerationForce = groundedData.StopData.SprintDecelerationForce;

        stateMachine.Current.JumpForce = airborneData.JumpData.SprintJumpForce;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.HardStopParameterHash);
    }
    #endregion

    #region 재사용 메소드
    protected override void OnMove()
    {
        if (stateMachine.Current.IsWalk)
        {
            return;
        }

        stateMachine.ChangeState(stateMachine.Run);
    }
    #endregion
}
