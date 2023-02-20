using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementState : EnemyStateCore
{
    protected MovementMathUtillity movementUtil;
    protected EnemyMovementData movementData;

    #region 생성자
    public EnemyMovementState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
        movementData = stateMachine.Enemy.Data.MovementData;      
        movementUtil = new MovementMathUtillity();
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        SetAnimationIntValue("animation", (int)EnemyAnimationType.Move);
        if (agent.isStopped)
        {
            agent.isStopped = false;
        }
        agent.speed = movementData.BaseMovementSpeed;
        Rotate(stateMachine.Current.KeepAgentVelocity);
        stateMachine.Current.PathCount = 0;
    }

    public override void PhysicalProcess()
    {
        Move();
    }
    #endregion

    #region 메인 메소드
    // 목표 각도로 현재 각도값 갱신
    private void UpdateRotationData(float targetAngle)
    {
        stateMachine.Current.CurrentTargetRotation.y = targetAngle;
        stateMachine.Current.DampedTargetRotationPassedTime.y = 0.0f;
    }
    #endregion

    #region 재사용 메소드
    protected Vector3 GetMovementHorizontalDirection(Vector3 start, Vector3 end, out float distance)
    {
        Vector3 result = end - start;
        result.y = 0.0f;
        distance = result.magnitude;
        return result.normalized;
    }

    protected void SetRotationData()
    {
        stateMachine.Current.RotationData = movementData.Rotation;

        stateMachine.Current.TimeToReachTargetRotation = stateMachine.Current.RotationData.ReachTargetRotation;
    }

    // 각도값 갱신
    protected float UpdateTargetRotation(Vector3 direction)
    {
        float angle = movementUtil.GetTargetAtanAngle(direction); // 이동방향 각도값

        // 각도값이 현재 바라보는 방향의 각도와 다를 경우
        if (angle != stateMachine.Current.CurrentTargetRotation.y)
        {
            UpdateRotationData(angle);
        }

        return angle;
    }

    // 목표 방향으로 회전
    protected void RotateToTargetRotation()
    {
        float currentY = stateMachine.Enemy.Rigid.rotation.eulerAngles.y; // 현재 y축 회전값

        if (currentY == stateMachine.Current.CurrentTargetRotation.y)
        {
            return;
        }
        float smoothY = Mathf.SmoothDampAngle(currentY,
            stateMachine.Current.CurrentTargetRotation.y, ref stateMachine.Current.DampedTargetRotationCurrentValocity.y,
            stateMachine.Current.TimeToReachTargetRotation.y - stateMachine.Current.DampedTargetRotationPassedTime.y);

        stateMachine.Current.DampedTargetRotationPassedTime.y += Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0.0f, smoothY, 0.0f);
        stateMachine.Enemy.Rigid.MoveRotation(targetRotation);
    }
    #endregion

    #region 재사용 가상 메소드

    protected virtual void UpdatePath(float distance)
    {
        if (distance < 0.1f)
        {
            if (stateMachine.Current.PathCount < stateMachine.Enemy.RoutinePath.Length - 1)
            {
                stateMachine.Current.PathCount++;
            }
            else
            {
                stateMachine.Current.PathCount = 0;
            }
        }
    }

    protected virtual void Move()
    {

    }

    protected virtual float Rotate(Vector3 direction)
    {
        float angle = UpdateTargetRotation(direction);
        RotateToTargetRotation();
        return angle;
    }
    #endregion
}
