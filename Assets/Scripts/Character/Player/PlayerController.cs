// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [field:Header("참조 데이터")]
    [field: SerializeField] public PlayerScriptableData Data { get; private set; } // 플레이어 스크립터블 오브젝트 데이터

    [field: Header("콜라이더")]
    [field: SerializeField] public CapsuleColliderUtil ColliderUtil { get; private set; } // 콜라이더 설정 유틸리티
    [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

    public PlayerInput Input { get; private set; } // 뉴 인풋 매니저에의한 입력을 관리하는 인스턴스
    public Rigidbody Rigid { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    public Transform MainCameraTrans { get; private set; } // 카메라 트랜스폼

    private PlayerMovementStateMachine movementStateMachine; // 이동관련 상태기계 필드

    private void Awake()
    {
        MainCameraTrans = Camera.main.transform;
        SetRigidbody();
        Input = GetComponent<PlayerInput>();
        movementStateMachine = new PlayerMovementStateMachine(this); // 상태기계 인스턴스 생성

    }

    private void OnValidate()
    {
        ColliderUtil.Initialize(gameObject);
        ColliderUtil.CalCollierDimension();
    }

    private void Start()
    {
        movementStateMachine.ChangeState(movementStateMachine.IdlingState); // 시작 시 Idling상태로 전환
    }

    private void Update()
    {
        movementStateMachine.HandleInput(); // 입력 처리
        movementStateMachine.Process(); // 동작 처리
    }

    private void FixedUpdate()
    {
        movementStateMachine.PhysicalProcess(); // 물리 동작 처리
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
    }

    [ContextMenu("SetDefaultColliderData")]
    private void SetDefaultColliderData()
    {
        ColliderUtil.CalColliderData(gameObject);
    }

}
