using UnityEngine;

public class EnemyTargetChasingMovementState : EnemyMovementState
{
    public EnemyTargetChasingMovementState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        stateMachine.Current.IsChasing = true;
        stateMachine.Current.Delta = 0.0f;
    }
    #endregion

    #region 재사용 메소드
    protected override void Move()
    {
        base.Move();
        float distance = Vector3.Distance(stateMachine.Enemy.transform.position, stateMachine.Current.TargetTrans.position);

        Rotate(agent.velocity.normalized);
        agent.SetDestination(stateMachine.Current.TargetTrans.position);
        // 플레이어와의 거리가 2 미만인 경우
        if(distance <= stateMachine.Enemy.GetAttackRange())
        {
            stateMachine.ChangeState(stateMachine.Attack);
        }
        else if(distance > 8.0f)
        {
            stateMachine.Current.Delta += Time.deltaTime;
            if (stateMachine.Current.Delta > 3.0f)
            {
                switch (stateMachine.Enemy.Data.MovementData.Type)
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
        }
    }
    #endregion
}
