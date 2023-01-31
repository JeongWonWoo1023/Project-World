using System;
using UnityEngine;

[Serializable]
public struct ItemStatusData
{
    [field: SerializeField][field: Tooltip("기본 공격력")] public float Attack { get; set; }

    [field: SerializeField][field: Tooltip("물리 관통력")] public float PhysicalPenetration { get; set; }
    [field: SerializeField][field: Tooltip("마법 관통력")] public float MagicalPenetration { get; set; }

    [field: SerializeField][field: Tooltip("치명타 확률")] public float CriticalProbability { get; set; }
    [field: SerializeField][field: Tooltip("치명타 데미지 계수")] public float CriticalDamageCoef { get; set; }

    [field: SerializeField][field: Tooltip("물리 방어력")] public float PhysicalDefence { get; set; }
    [field: SerializeField][field: Tooltip("마법 저항력")] public float MagicalDefence { get; set; }

    [field: SerializeField][field: Tooltip("기본 체력")] public int MaxHp { get; set; }
    [field: SerializeField][field: Tooltip("레벨당 추가 체력")] public int HpCoef { get; set; }

    [field: SerializeField][field: Tooltip("기본 마나")] public int MaxMp { get; set; }
    [field: SerializeField][field: Tooltip("레벨당 추가 마나")] public int MpCoef { get; set; }
}
