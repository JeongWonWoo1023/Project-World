// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [field:Header("참조 데이터")]
    [field: SerializeField] public PlayerScriptableData Data { get; private set; } // 플레이어 스크립터블 오브젝트 데이터
    public PlayerInput Input { get; private set; } // 뉴 인풋 매니저에의한 입력을 관리하는 인스턴스
    public Rigidbody Rigid { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    public Transform MainCameraTrans { get; private set; } // 카메라 트랜스폼

    private PlayerMovementStateMachine movementStateMachine; // 이동관련 상태기계 필드

    private void Awake()
    {
        MainCameraTrans = Camera.main.transform;
        movementStateMachine = new PlayerMovementStateMachine(this); // 상태기계 인스턴스 생성

    }

    private void Start()
    {
        SetRigidbody();
        SetCollider();
        Input = GetComponent<PlayerInput>();
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

    // 캡슐 콜라이더 컴포넌트 설정
    [ContextMenu("SetCollider")]
    private void SetCollider()
    {
        Collider = GetComponent<CapsuleCollider>();
        if(Collider == null)
        {
            Collider = gameObject.AddComponent<CapsuleCollider>();

            float maxHeight = -1.0f;

            SkinnedMeshRenderer[] skinMeshs = GetComponentsInChildren<SkinnedMeshRenderer>(); // 스킨 메시 랜더러 컴포넌트 모두 불러오기

            // 컴포넌트가 하나라도 있는 경우
            if(skinMeshs.Length > 0)
            {
                foreach(SkinnedMeshRenderer mesh in skinMeshs)
                {
                    foreach(Vector3 vertex in mesh.sharedMesh.vertices)
                    {
                        if(maxHeight < vertex.y)
                        {
                            maxHeight= vertex.y;
                        }
                    }
                }
            }
            // 스킨 메시 랜더러가 없을 경우
            else
            {
                MeshFilter[] meshFIlters = GetComponentsInChildren<MeshFilter>(); // 메시 필터 컴포넌트 모두 불러오기

                // 컴포넌트가 하나라도 있는 경우
                if(meshFIlters.Length > 0)
                {
                    foreach(MeshFilter mesh in meshFIlters)
                    {
                        foreach(Vector3 vertex in mesh.mesh.vertices)
                        {
                            if(maxHeight < vertex.y)
                            {
                                maxHeight = vertex.y;
                            }
                        }
                    }
                }
            }
            if (maxHeight <= 0)
            {
                maxHeight = 1.0f;
            }
            float center = maxHeight * 0.5f;
            Collider.height = maxHeight;
            Collider.center = Vector3.up * center;
            Collider.radius = 0.2f;
        }
    }
}
