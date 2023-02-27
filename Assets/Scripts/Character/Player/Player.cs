// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : CharacterBattle
{
    public string nickName;

    [field:Header("참조 데이터")]
    [field: SerializeField] public PlayerScriptableData Data { get; private set; } // 이동 관련 데이터
    [field: SerializeField] public NearObjectDetector NearDetector { get; private set; } // 아이템 감지기
    [field: SerializeField] public Transform Hand { get; private set; } // 이펙트 위치, 회전 적용을 위한 손 트랜스폼
    [field: SerializeField] public SkiilData NormalAttack { get; private set; }
    [field: SerializeField] public SkiilData StrongAttack { get; private set; }
    [field: SerializeField] public SkiilData Ultimadom { get; private set; }

    [field: Header("콜라이더")]
    [field: SerializeField] public PlayerCapsuleColliderUtil ColliderUtil { get; private set; } // 콜라이더 설정 유틸리티
    [field: SerializeField] public LayerData LayerData { get; private set; }

    [field: Header("카메라")]
    [field: SerializeField] public PlayerCameraUtil CameraUtil { get; private set; } // 카메라 유틸리티
    [field: SerializeField] public CinemachineInputProvider CameraInput { get; private set; } // 카메라 입력 컴포넌트

    [field: Header("애니메이션")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; } // 애니메이션 데이터

    [field: Header("무장")]
    [field: SerializeField] public Item Weapon { get; private set; } // 무기

    public PlayerInput Input { get; private set; } // 뉴 인풋 매니저에의한 입력을 관리하는 인스턴스
    public Rigidbody Rigid { get; private set; }
    public Animator Anim { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    public Transform MainCameraTrans { get; private set; } // 카메라 트랜스폼

    private PlayerStateMachine _stateMachine;
    public PlayerStateMachine stateMachine
    {
        // 상태기계
        get
        {
            if (_stateMachine == null)
            {
                _stateMachine = new PlayerStateMachine(this);
            }
            return _stateMachine;
        }
    }

    private PlayerSaveData _saveData;

    private void Awake()
    {
        MainCameraTrans = Camera.main.transform;
        SetRigidbody();
        Anim = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerInput>();

        ColliderUtil.Initialize(gameObject);
        ColliderUtil.CalCollierDimension();
        CameraUtil.Initialize();
        AnimationData.Initialize();
        hitSound = Data.HitSound;

        MainCameraTrans = Camera.main.transform;

        // 스탯 초기화
        PlayerSaveData data = LoadStatus();
        if(data == null)
        {
            nickName = "닉네임은최대12글자까지";
        }
        else
        {
            nickName = data.nickName;
        }
        if (data == null)
        {
            InitializeStatus(Data.DefualtStatus);
        }
        else
        {
            UpdateStatus(data.status, data.level, data.currentHP, data.currentMP, data.currentEXP);
        }
    }

    private void OnValidate()
    {
        ColliderUtil.Initialize(gameObject);
        ColliderUtil.CalCollierDimension();
    }

    private void Start()
    {
        stateMachine.ChangeState(stateMachine.Idle); // 시작 시 Idling상태로 전환
    }

    private void OnTriggerEnter(Collider other)
    {
        stateMachine.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        stateMachine.OnTriggerExit(other);
    }

    private void Update()
    {
        SaveStatus();
        stateMachine.HandleInput(); // 입력 처리
        stateMachine.Process(); // 동작 처리
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicalProcess(); // 물리 동작 처리
    }

    // 리지드바디 컴포넌트 설정
    [ContextMenu("SetRigidbody")]
    private void SetRigidbody()
    {
        Rigid = GetComponent<Rigidbody>();
        if(Rigid== null)
        {
            Rigid = gameObject.AddComponent<Rigidbody>();
        }
        Rigid.constraints = RigidbodyConstraints.FreezeRotation; // 물리 회전 제한
        Rigid.interpolation = RigidbodyInterpolation.Interpolate; // 현재 프레임과 다음 프레임 사이 추정값 보간옵션
        Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌 추정 옵션
        Rigid.useGravity = false; // 중력 수동조절을 위해 비활성
    }

    [ContextMenu("SetDefaultColliderData")]
    private void SetDefaultColliderData()
    {
        ColliderUtil.CalColliderData(gameObject);
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

    public void OnAttackStateAnimationEvent()
    {
        stateMachine.OnAnimationAttackEvent();
    }

    private void SaveStatus()
    {
        // 스탯 데이터 세이브
        if(_saveData == null)
        {
            _saveData = new PlayerSaveData();
        }
        _saveData.nickName = nickName;
        _saveData.status = status;
        _saveData.level = Level;
        _saveData.currentHP = CurrentHP;
        _saveData.currentMP = CurrentMP;
        _saveData.currentEXP = CurrentEXP;
        DataManager.Instance.SaveJson(_saveData,typeof(PlayerSaveData).Name);
    }

    private PlayerSaveData LoadStatus()
    {
        // 스탯 데이터 로드
        PlayerSaveData result = DataManager.Instance.LoadJson<PlayerSaveData>(typeof(PlayerSaveData).Name);
        if (result == null)
        {
            return null;
        }
        return result;
    }
}
