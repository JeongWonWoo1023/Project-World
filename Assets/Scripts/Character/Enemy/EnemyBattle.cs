using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattle : BattleSystem
{
    [Serializable]
    public struct EnemyStatus
    {
        public EnemyStatusData defualtData; // 기본 데이터

        [Space, Header("변화량 데이터")]
        public int level; // 레벨
        public int maxHP; // 최대 체력
        public int currentHP; // 현재 체력
        public int attack; // 공격력
        public int defense; // 방어력

        // 몬스터가 메모리풀에 적재되거나 월드레벨이 증가하면 호출
        public void InitStatus()
        {
            level = defualtData.level; // 월드레벨에 따라 증가하는 로직 필요
            currentHP = maxHP = defualtData.hp + defualtData.hp * (int)(level * 0.5f); // 최대체력 = 기본 체력 * 몬스터 레벨의 절반 ( 소수점 버림 )
            attack = defualtData.attack * (int)(level * 0.2f); // 공격력 = 기본 공격력 * 몬스터 레벨의 20%
            defense = defualtData.defense * (int)(level * 0.15f); // 방어력 = 기본 방어력 * 몬스터 레벨의 15%
        }
    }

    [SerializeField] private EnemyStatus stat = new EnemyStatus();
    [SerializeField] private LayerMask targetMask;

    private Collider[] collTargets = default;

    protected override void Start()
    {
        base.Start();
        stat.InitStatus();
    }

    public override void OnDamage(int value)
    {
        stat.currentHP -= value;
        State.isDead = stat.currentHP <= 0;
        if (State.isDead)
        {
            stat.currentHP = 0;
        }
    }
}
