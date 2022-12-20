using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public abstract class EnemyRequire : MonoBehaviour
{
    [Serializable]
    public class Components
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public IMovement movement;
        [HideInInspector] public INode behaviorRoot;
    }

    [Serializable]
    public class EnemyState
    {
        public bool isSpwan; // 스폰 단계
        public bool isIdle; // 일반 상태
        public bool isDead; // 사망 단계
        public bool isMoving; // 이동중
        public bool isRunning; // 달리기 중
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

    [SerializeField] private Components _compo = new Components();
    [SerializeField] private EnemyState _state = new EnemyState();
    [SerializeField] private AnimatorOption _animOption = new AnimatorOption();

    [SerializeField] protected Vector3[] movePath;

    public Components Compo => _compo;
    public EnemyState State => _state;
    public AnimatorOption AnimOption => _animOption;

    protected virtual void InitializeComponenet()
    {
        Compo.anim = GetComponentInChildren<Animator>();
        TryGetComponent(out Compo.movement);
    }

    // 비헤이비어트리 생성 추상메소드
    protected abstract void SetBehavior();
}
