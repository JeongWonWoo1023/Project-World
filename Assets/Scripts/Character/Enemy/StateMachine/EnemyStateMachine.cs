using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    #region 필드 & 프로퍼티
    public Enemy Enemy { get; } // 적 메인 스크립트
    public EnemyCurrentData Current { get; } // 실시간 데이터

    public EnemySpawnState Spawn { get; } // 스폰
    public EnemySelectionPathMovementState SelectionPathMove { get; } // 선택 경로 이동
    public EnemyRandomPathMovementState RandomPathMove { get; } // 무작위 경로 이동
    public EnemyTargetChasingMovementState TargetChasingMove { get; } // 추적 이동
    public EnemyAttackState Attack { get; } // 공격
    public EnemyHitState Hit { get; } // 피격
    public EnemyDeadState Dead { get; } // 사망
    #endregion

    #region 생성자
    public EnemyStateMachine(Enemy enemy)
    {
        Enemy = enemy;
        Current = new EnemyCurrentData();
        Spawn = new EnemySpawnState(this);
        SelectionPathMove = new EnemySelectionPathMovementState(this);
        RandomPathMove = new EnemyRandomPathMovementState(this);
        TargetChasingMove = new EnemyTargetChasingMovementState(this);
        Attack = new EnemyAttackState(this);
        Hit = new EnemyHitState(this);
        Dead = new EnemyDeadState(this);
    }
    #endregion
}
