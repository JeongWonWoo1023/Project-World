using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Status Data", menuName = "Scriptable Object/Enemy Data", order = int.MaxValue)]
public class EnemyStatusData : ScriptableObject
{
    public enum AttackType
    {
        Physical, Magical
    }

    [Range(1,100),Tooltip("기본 레벨")] public int level = 1;
    [Range(1, 100000), Tooltip("기본 공격력")] public int attack = 1;
    [Range(1, 100000), Tooltip("기본 방어력")] public int defense = 1;
    [Range(1, 100000), Tooltip("기본 체력")] public int hp = 1;

    [Tooltip("공격 유형")] public AttackType attackType = AttackType.Physical;
    [Range(0.0f, 1.0f), Tooltip("물리 방어율")] public float physicalDefenseRatio = 0.0f;
    [Range(0.0f, 1.0f), Tooltip("마법 방어율")] public float magicalDefenseRatio = 0.0f;
}
