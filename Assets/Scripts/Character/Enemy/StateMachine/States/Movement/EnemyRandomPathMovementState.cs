using UnityEngine;
using UnityEngine.AI;

public class EnemyRandomPathMovementState : EnemyMovementState
{
    private Vector3 currentPath;
    public EnemyRandomPathMovementState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {

    }

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        RandomAgentPath(10, out currentPath);
    }
    #endregion

    private bool RandomAgentPath(float range, out Vector3 result)
    {
        for(int i = 0; i < 30; ++i)
        {
            Vector3 point = stateMachine.Enemy.SpawnPoint.transform.position + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(point,out hit,1.0f,NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    protected override void Move()
    {
        base.Move();
        stateMachine.Current.KeepAgentVelocity = GetMovementHorizontalDirection(stateMachine.Enemy.transform.position, currentPath, out float distance);
        Vector3 currentDirection = stateMachine.Current.Direction;

        if (currentDirection != stateMachine.Current.KeepAgentVelocity)
        {
            // 다음 이동 방향으로 회전
            Rotate(stateMachine.Current.KeepAgentVelocity);
        }

        stateMachine.Current.Direction = stateMachine.Current.KeepAgentVelocity;
        // 이동속도 값 대입 필요
        agent.SetDestination(currentPath);
        UpdatePath(distance);
    }

    protected override void UpdatePath(float distance)
    {
        if (distance < 0.1f)
        {
            RandomAgentPath(10, out currentPath);
        }
    }
}
