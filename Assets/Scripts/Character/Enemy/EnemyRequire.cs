using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using static BehaviorTree.NodeCreator;

public abstract class EnemyRequire : MonoBehaviour
{
    [Serializable]
    public class Components
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public IEnemyMovement movement;
        [HideInInspector] public INode behaviorRoot;
    }

    [Serializable]
    public class StatusOption
    {
        public EnemyStatusData defualtData; // 기본 데이터

        [Space, Header("변화량 데이터")]
        public int level = 0; // 레벨
        public int maxHP = 0; // 최대 체력
        public int currentHP = 0; // 현재 체력
        public int attack = 0; // 공격력
        public int defense = 0; // 방어력

        // 몬스터가 메모리풀에 적재되거나 월드레벨이 증가하면 호출
        public void SetStatus()
        {
            level = defualtData.level; // 월드레벨에 따라 증가하는 로직 필요
            currentHP = maxHP = defualtData.hp * (int)(level * 0.5f); // 최대체력 = 기본 체력 * 몬스터 레벨의 절반 ( 소수점 버림 )
            attack = defualtData.attack * (int)(level * 0.2f); // 공격력 = 기본 공격력 * 몬스터 레벨의 20%
            defense = defualtData.defense * (int)(level * 0.15f); // 방어력 = 기본 방어력 * 몬스터 레벨의 15%
        }
    }

    [Serializable]
    public class EnemyState
    {
        public bool isSpwan; // 스폰 단계
        public bool isIdle; // 일반 상태
        public bool isDead; // 사망 단계
        public bool isMoving; // 이동중
        public bool isBattle; // 전투 상태
        public bool isAttack; // 공격 중
        public bool isHit; // 공격당하는 중
        public bool isGrounded; // 지면 접지
    }

    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveZ = "Move Z";
        public string paramDistY = "Dist Y";
        public string paramGrounded = "Grounded";
        public string paramHit = "Hit";
        public string paramAttack = "Attack";
    }

    [SerializeField] Components _compo = new Components();
    [SerializeField] StatusOption _stat = new StatusOption();
    [SerializeField] EnemyState _state = new EnemyState();
    [SerializeField] AnimatorOption _animOption = new AnimatorOption();

    public Components Compo => _compo;
    public StatusOption Stat => _stat;
    public EnemyState State => _state;
    public AnimatorOption AnimOption => _animOption;

    // 비헤이비어트리 생성 추상메소드
    protected abstract void SetBehavior(INode root);
}
