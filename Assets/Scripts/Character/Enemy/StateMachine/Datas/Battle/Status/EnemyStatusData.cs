using System;
using UnityEngine;

[Serializable]
public class EnemyStatusData : StatusData
{
    [field: SerializeField][field: Tooltip("적 유형")] public EnemyAttackType AttackDistanceType { get; set; } = EnemyAttackType.MeleeAttack;
}
