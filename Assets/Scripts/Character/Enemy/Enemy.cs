using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : CharacterBattle
{
    [field: Header("참조 데이터")]
    [field: SerializeField] public EnemyScriptableData Data { get; private set; }
    [field: SerializeField] public ItemDropTable DropTable { get; private set; }
    [field: SerializeField] public FindTargetUtility FindTargetUtility { get; private set; }
    [field: SerializeField] public Transform UITrakingOffset { get; private set; }
    private EnemyStateMachine _stateMachine;

    public Animator Anim { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public NavMeshAgent NavAgent { get; private set; }
    public CapsuleCollider Collider { get; private set; }

    public EnemyStateMachine stateMachine 
    {
        get
        {
            if(_stateMachine == null)
            {
                _stateMachine = new EnemyStateMachine(this);
            }
            return _stateMachine;
        }
    }

    public Vector3[] RoutinePath { get; set; }

    public Action AttackAction;

    private void OnDisable()
    {
        ObjectPool.Instance.PushPool(gameObject);
        CancelInvoke();
    }

    private void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        InitializeStatus(Data.DefualtStatus);
        EnemyUITrakingTrans = UITrakingOffset;
        Anim = GetComponentInChildren<Animator>();
        SetRigidbody();
    }

    private void Update()
    {
        stateMachine.Process(); // 프레임당 동작 처리
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicalProcess(); // 물리 동작 처리
    }

    private void OnTriggerEnter(Collider other)
    {
        stateMachine.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        stateMachine.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        stateMachine.OnTriggerExit(other);
    }

    // 리지드바디 컴포넌트 설정
    [ContextMenu("SetRigidbody")]
    private void SetRigidbody()
    {
        Rigid = GetComponent<Rigidbody>();
        if (Rigid == null)
        {
            Rigid = gameObject.AddComponent<Rigidbody>();
        }
        Rigid.constraints = RigidbodyConstraints.FreezeRotation; // 물리 회전 제한
        Rigid.interpolation = RigidbodyInterpolation.Interpolate; // 현재 프레임과 다음 프레임 사이 추정값 보간옵션
        Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌 추정 옵션
        Rigid.isKinematic = true;   
    }

    public void OnMovementStateAnimationEnter()
    {
        stateMachine.OnAnimationEnterEvent();
    }

    public void OnMovementStateAnimationExit()
    {
        stateMachine.OnAnimationExitEvent();
    }

    public void OnMovementStateAnimationTransition()
    {
        stateMachine.OnAnimationTransitionEvent();
    }
}
