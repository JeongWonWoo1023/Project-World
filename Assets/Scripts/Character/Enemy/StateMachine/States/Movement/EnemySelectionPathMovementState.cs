using UnityEngine;

public class EnemySelectionPathMovementState : EnemyMovementState
{

    public EnemySelectionPathMovementState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {

    }

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        targetDirection = GetMovementHorizontalDirection(stateMachine.Enemy.transform.position, stateMachine.Enemy.RoutinePath[stateMachine.Current.PathCount], out _);
    }

    public override void Exit()
    {
        base.Exit();
        agent.isStopped= true;
        agent.velocity = Vector3.zero;
    }
    #endregion

    #region 재사용 메소드
    protected override void Move()
    {
        base.Move();
        targetDirection = GetMovementHorizontalDirection(stateMachine.Enemy.transform.position, stateMachine.Enemy.RoutinePath[stateMachine.Current.PathCount], out float distance);
        Vector3 currentDirection = stateMachine.Current.Direction;

        if (currentDirection != targetDirection)
        {
            // 다음 이동 방향으로 회전
            Rotate(targetDirection);
        }

        stateMachine.Current.Direction = targetDirection;
        // 이동속도 값 대입 필요
        agent.SetDestination(stateMachine.Enemy.RoutinePath[stateMachine.Current.PathCount]);
        UpdatePath(distance);
    }
    #endregion
}
