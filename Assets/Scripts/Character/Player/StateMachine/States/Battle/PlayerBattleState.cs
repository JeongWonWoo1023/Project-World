using UnityEngine.InputSystem;

public class PlayerBattleState : PlayerStateCore
{
    #region 생성자
    public PlayerBattleState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        stateMachine.Current.IsAttak = true;
        StartAnimation(stateMachine.Player.AnimationData.BattleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Current.IsAttak = false;
        StopAnimation(stateMachine.Player.AnimationData.BattleParameterHash);
    }
    #endregion

    #region 재사용 메소드
    // 입력 콜백 등록
    protected override void AddInputAction()
    {
        base.AddInputAction();
    }

    // 입력 콜백 등록 해제
    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
    }
    #endregion

    #region 입력 메소드

    protected virtual void OnStrongAttack(InputAction.CallbackContext obj)
    {
        // 강공격 상태로 전환
    }
    #endregion
}
