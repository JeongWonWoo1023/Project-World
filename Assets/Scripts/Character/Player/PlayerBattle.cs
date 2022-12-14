using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattle : BattleSystem
{
    [Serializable]
    public struct PlayerStatus
    {
        public PlayerStatusData defualtData; // 캐릭터 기본 값

        [Space, Header("레벨 & 경험치 데이터")]
        public int level;
        public int maxEXP;
        public int currentEXP;

        [Space, Header("체력, 마나, 기력 데이터")]
        public int maxHP;
        public int currentHP;
        public int maxMP;
        public int currentMP;
        public int maxStamina;
        public int currentStamina;

        [Space, Header("전투 능력치 데이터")]
        public int attack;
        public int defense;

        // 최초 로드 시 호출
        public void InitStatus()
        {
            level = 1; // 데이터 연동 필요 ( 임시 값 : 1 )

            // 데이터 연동 시 직전 접속 기록을 따라야 함
            currentHP = maxHP = defualtData.hp * (int)(level * 0.5f);
            currentMP = maxMP = defualtData.mp * (int)(level * 0.5f);
            currentStamina = maxStamina = defualtData.stamina * (int)(level * 0.01f);
            attack = defualtData.attack * (int)(level * 0.2f);
            defense = defualtData.defense * (int)(level * 0.15f);
        }
    }

    [SerializeField] private PlayerStatus stat = new PlayerStatus();
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
            OnDead();
        }
    }

    // 각 이벤트별 동작 메소드 정의

    public void Attack()
    {
        // 물리공격, 마법공격 판정 분리
        switch (stat.defualtData.attackType)
        {
            case AttackType.Physical:
                collTargets = Physics.OverlapSphere(Compo.center, Compo.radius, targetMask, QueryTriggerInteraction.Ignore);
                break;
            case AttackType.Magical:
                if(Compo.collSphere != null)
                {
                    collTargets = Physics.OverlapSphere(Compo.center, Compo.radius, targetMask, QueryTriggerInteraction.Ignore);
                }
                else if(Compo.collBox != null)
                {
                    collTargets = Physics.OverlapBox(Compo.center, Compo.boxSize, Quaternion.identity, targetMask, QueryTriggerInteraction.Ignore);
                }
                break;
        }

        if(collTargets.Length > 0)
        {
            foreach (IBattle target in collTargets)
            {
                target?.OnDamage(10);
            }
        }
    }
}
