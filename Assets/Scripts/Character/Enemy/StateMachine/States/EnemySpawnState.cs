using UnityEngine;

public class EnemySpawnState : EnemyStateCore
{
    #region 생성자
    public EnemySpawnState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        Initialize();
    }
    #endregion

    #region 메인 메소드
    private void TypeToTransition()
    {
        switch(stateMachine.Enemy.Data.MovementData.Type)
        {
            case EnemyMovementType.Stand:
                break;
            case EnemyMovementType.SelectPath:
                stateMachine.ChangeState(stateMachine.SelectionPathMove);
                break;
            case EnemyMovementType.RandomPath:
                stateMachine.ChangeState(stateMachine.RandomPathMove);
                break;
        }
    }
    #endregion

    #region 재사용 메소드
    // 오브젝트 재사용 시 초기화
    protected virtual void Initialize()
    {
        stateMachine.Enemy.IsDead = false; // 사망하지 않은 상태로 초기화
        stateMachine.Current.PathCount = 0; // 이동 패스 초기화
        stateMachine.Enemy.gameObject.SetActive(true);
        TypeToTransition();
    }

    // 사망 시 수행
    protected virtual void Dead()
    {
        stateMachine.Enemy.IsDead = true;
    }
    #endregion
}
