using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateCore : StateCore
{
    #region 필드 & 프로퍼티
    protected EnemyStateMachine stateMachine;
    protected NavMeshAgent agent;
    #endregion

    #region 생성자
    public EnemyStateCore(EnemyStateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
        agent = stateMachine.Enemy.NavAgent;
        agent.updateRotation = false;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Process()
    {
        base.Process();
        if(stateMachine.Enemy.IsHit)
        {
            stateMachine.Enemy.IsHit = false;
            stateMachine.ChangeState(stateMachine.Hit);
        }
        if(stateMachine.Enemy.IsDead)
        {
            stateMachine.Current.IsDead = true;
            stateMachine.Enemy.IsDead = false;
            stateMachine.ChangeState(stateMachine.Dead);
        }
    }

    public override void OnTriggerStay(Collider collider)
    {
        if (stateMachine.Enemy.FindTargetUtility.TargetMask.value != 1 << collider.gameObject.layer)
        {
            return;
        }
        stateMachine.Current.TargetTrans = collider.transform;
        Vector3 direction = (stateMachine.Current.TargetTrans.position -
            stateMachine.Enemy.transform.position + Vector3.up *
            stateMachine.Enemy.FindTargetUtility.FindCollider.center.y).normalized;

        // 기즈모 앵글이 좌, 우 벡터의 각도이므로 중앙 벡터를 기준으로한 각에서 2배 곱하는 연산 필요
        bool isHorizontal = GetHorizontalAngle(direction) * 2 <= stateMachine.Enemy.FindTargetUtility.HorizontalAngle;
        bool isvertical = GetVerticalAngle(direction) * 2 <= stateMachine.Enemy.FindTargetUtility.VerticalAngle;
        // 제한각에 들어왔을 경우
        if (isHorizontal && isvertical && !stateMachine.Current.IsChasing)
        {
            stateMachine.Current.Delta += Time.deltaTime;
            // 1초 유지가 되었을 경우
            if(Mathf.Approximately(stateMachine.Current.Delta, 1.0f))
            {
                stateMachine.ChangeState(stateMachine.TargetChasingMove);
            }
        }
        else if(stateMachine.Current.Delta > 0.0f && !stateMachine.Current.IsChasing)
        {
            stateMachine.Current.Delta = 0.0f;
        }
    }
    #endregion

    #region 메인 메소드 ( 모든 상태에서 실행 )
    private float GetHorizontalAngle(Vector3 targetDirection)
    {
        Vector3 direction = targetDirection;
        direction.y = 0.0f;
        float dot = Vector3.Dot(stateMachine.Enemy.transform.forward, direction);
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }

    private float GetVerticalAngle(Vector3 targetDirection)
    {
        Vector3 direction = targetDirection;
        direction.x = 0.0f;
        direction.z = 0.0f;
        float dot = Vector3.Dot(stateMachine.Enemy.transform.up, direction);
        return Mathf.Abs(dot * Mathf.Rad2Deg);
    }
    #endregion

    #region 재사용 메소드
    // 모션 시작
    protected override void StartAnimation(int hash)
    {
        stateMachine.Enemy.Anim.SetBool(hash, true);
    }

    // 모션 종료
    protected override void StopAnimation(int hash)
    {
        stateMachine.Enemy.Anim.SetBool(hash, false);
    }

    // int값에 의한 모션 시작
    protected override void SetAnimationValueFromHash(int hash, int value)
    {
        stateMachine.Enemy.Anim.SetInteger(hash, value);
    }

    protected void SetAnimationIntValue(string name, int value)
    {
        stateMachine.Enemy.Anim.SetInteger(name, value);
    }
    #endregion
}
