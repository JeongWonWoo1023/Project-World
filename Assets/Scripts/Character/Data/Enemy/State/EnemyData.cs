using System;
using UnityEngine;

public enum EnemyMovementType
{
    Stand, SelectPath, RandomPath
}

[Serializable]
public class EnemyMovementData
{
    [field: SerializeField]
    [field: Tooltip("이동 유형 ( 제자리 정지, 선택 경로 이동, 랜덤 경로 이동 )")]
    public EnemyMovementType Type;

    [field: SerializeField]
    [field:Tooltip("0 ~ 90도 간 경사로의 이동속도 계수값 설정 ( 값은 0 ~ 1 사이 실수값 )")]
    public AnimationCurve SlopeMovementCoef { get; private set; }

    [field: SerializeField]
    [field: Range(0.0f, 5.0f)]
    [field: Tooltip("기본 이동속도")]
    public float BaseMovementSpeed { get; private set; } = 3.5f;

    [field: SerializeField]
    [field: Range(0.0f, 2.0f)]
    [field: Tooltip("적 추적 시 이동속도 계수")]
    public float ChasingMovementCoef { get; private set; } = 1.5f;

    [field: SerializeField]
    [field: Range(0.0f, 50.0f)]
    [field: Tooltip("추적 거리유지 값")]
    public float KeepDistance { get; private set; } = 1.5f;

    [field: SerializeField]
    [field: Range(10.0f, 100.0f)]
    [field: Tooltip("추적 해제 거리")]
    public float DisableChasingDistance { get; private set; } = 15.0f;

    [field: SerializeField]
    [field: Tooltip("회전 속도 값 ( Y값 = 목표 방향으로의 회전 시간 )")]
    public RotationData Rotation { get; private set; }

    [field: SerializeField]
    [field: Tooltip("스폰 시점 모션")]
    public EnemyAnimationType StartAnimation { get; private set; } = EnemyAnimationType.Idle;
}