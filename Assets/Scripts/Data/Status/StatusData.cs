using System;
using UnityEngine;

[Serializable]
public class StatusData
{
    [field: SerializeField] public ExperienceStatus Experience { get; private set; }
    [field: SerializeField][field: Tooltip("공격 유형")] public AttackType AttackType { get; set; } = AttackType.Physical;
    [field: SerializeField] public AttackStatus Attack { get; private set; }
    [field: SerializeField] public DefenceStatus Defence { get; private set; }
    [field: SerializeField] public HealthStatus Health { get; private set; }

    public StatusData()
    {
        Experience = new ExperienceStatus();
        Attack = new AttackStatus();
        Defence = new DefenceStatus();
        Health = new HealthStatus();
    }
}

[Serializable]
public class ExperienceStatus
{
    [field: SerializeField][field: Tooltip("레벨")] public int Level { get; set; } = 1;
    [field: SerializeField][field: Tooltip("플레이어 : 최대 경험치, 적 : 보상 경험치")] public int Point { get; set; } = 1000;
}

[Serializable]
public class AttackStatus
{
    [field: SerializeField][field: Tooltip("기본 공격력")] public float Defualt { get; set; } = 10.0f;
    [field: SerializeField][field: Tooltip("레벨당 공격력 계수")] public float Coef { get; set; } = 0.5f;

    [field: SerializeField][field: Tooltip("물리 관통력")] public float PhysicalPenetration { get; set; } = 0.1f;
    [field: SerializeField][field: Tooltip("마법 관통력")] public float MagicalPenetration { get; set; } = 0.1f;

    [field: SerializeField][field: Tooltip("치명타 확률")] public float CriticalProbability { get; set; } = 0.0f;
    [field: SerializeField][field: Tooltip("치명타 데미지 계수")] public float CriticalDamageCoef { get; set; } = 1.5f;
}

[Serializable]
public class DefenceStatus
{
    [field: SerializeField][field: Tooltip("기본 물리 방어력")] public float DefualtPhysical { get; set; } = 8.0f;
    [field: SerializeField][field: Tooltip("레벨당 물리 방어력 계수")] public float PhysicalCoef { get; set; } = 0.5f;

    [field: SerializeField][field: Tooltip("기본 마법 저항력")] public float DefualtMagical { get; set; } = 8.0f;
    [field: SerializeField][field: Tooltip("레벨당 마법 저항력 계수")] public float MagicalCoef { get; set; } = 0.5f;
}

[Serializable]
public class HealthStatus
{
    [field: SerializeField][field: Tooltip("기본 체력")] public int MaxHp { get; set; } = 500;
    [field: SerializeField][field: Tooltip("레벨당 체력 증가량")] public int HpCoef { get; set; } = 30;

    [field: SerializeField][field: Tooltip("기본 마나")] public int MaxMp { get; set; } = 300;
    [field: SerializeField][field: Tooltip("레벨당 마나 증가량")] public int MpCoef { get; set; } = 10;
}