using UnityEngine;

public enum EnemyAnimationType
{
    Create, Idle, Move, Attack, Damage, Die
}

public class EnemyCurrentData : CharacterCurrentData
{
    public RotationData RotationData { get; set; }

    public Vector3 SpawnPoint { get; set; } // 스폰 지점
    public Vector3 Direction { get; set; } // 다음 이동 목표지점 방향
    public Transform TargetTrans { get; set; } // 추적 대상 트랜스폼

    public int PathCount { get; set; } // 이동 목표 패스 카운트
    public float Delta { get; set; } // 델타타임 캐싱
    
    public bool IsMove { get; set; } // 이동
    public bool IsChasing { get; set; } // 추적
}
