using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Status Data", menuName = "Scriptable Object/Player Data", order = int.MaxValue)]
public class PlayerStatusData : ScriptableObject
{

    [Range(1, 100000), Tooltip("기본 공격력")] public int attack = 1;
    [Range(1, 100000), Tooltip("기본 방어력")] public int defense = 1;
    [Range(1, 100000), Tooltip("기본 체력")] public int hp = 1;
    [Range(1, 100000), Tooltip("기본 마력")] public int mp = 1;
    [Range(1, 100000), Tooltip("기본 기력")] public int stamina = 1;

    [Tooltip("공격 유형")] public AttackType attackType = AttackType.Physical;
    [Range(0.0f, 1.0f), Tooltip("물리 방어율")] public float physicalDefenseRatio = 0.0f;
    [Range(0.0f, 1.0f), Tooltip("마법 방어율")] public float magicalDefenseRatio = 0.0f;
}
