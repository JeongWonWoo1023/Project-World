using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer : Jeong Won Woo
// Create : 2022. 12. 05.
// Update : 2022. 12. 05.

public abstract class PlayerRequire : MonoBehaviour
{
    public enum CamType
    {
        Player
    }

    // 옵션 클래스
    [Serializable]
    public class Components
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigd;
        [HideInInspector] public CapsuleCollider capsule;

        public Camera cam;

        [HideInInspector] public Transform modelRoot;
        [HideInInspector] public Transform camRoot;
        [HideInInspector] public Transform camRig;
        [HideInInspector] public GameObject camObj;

        [HideInInspector] public IMovement movement;
    }

    [Serializable]
    public class CameraOption
    {
        [Tooltip("시작 시 카메라")]
        public CamType camType;

        [Range(1.0f, 10.0f), Tooltip("카메라 회전 속도")]
        public float rotateSpeed = 2.0f;

        [Range(-90.0f, 0.0f), Tooltip("올려다보기 제한 각")]
        public float lookUpLimitAngle = -60.0f;

        [Range(0.0f, 90.0f), Tooltip("내려다보기 제한 각")]
        public float lookDownLimitAngle = 75.0f;

        [Tooltip("지면으로 인식 할 레이어")]
        public LayerMask groundMask = -1;

        [Range(0.0f, 7.5f), Tooltip("줌인 최대 거리")]
        public float zoomInDistance = 3.0f;

        [Range(0.0f, 7.5f), Tooltip("줌아웃 최대 거리")]
        public float zoomOutDistance = 3.0f;

        [Range(1.0f, 30.0f), Tooltip("줌 속도")]
        public float zoomSpeed = 20.0f;

        [Range(0.01f, 0.5f), Tooltip("줌 보간 속도")]
        public float zoomAccel = 0.1f;
    }

    [Serializable]
    public class KeyOption
    {
        // 이동
        public KeyCode moveForward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;

        public KeyCode jump = KeyCode.Space; // 점프
        public KeyCode dash = KeyCode.LeftShift; // 대쉬
        public KeyCode walkMod = KeyCode.LeftControl; // 걷기모드 전환
    }

    [Serializable]
    public class StateOption
    {
        public bool isMoving; // 걷는 중
        public bool isRunning; // 뛰는 중
        public bool isGrounded; // 지면 접지
        public bool isUnableMoveSlope; // 걸어서 등반 불가능한 경사로에 있는 경우
        public bool isJumpTrig; // 점프 트리거
        public bool isJumping; // 점프 중
        public bool isBlocked; // 전방에 장애물 존재
        public bool isOutOfControl; // 제어불가 상태
        public bool isObstacle; // 카메라 루트와 카메라 사이에 장애물이 감지될 경우
    }

    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveZ = "Move Z";
        public string paramDistY = "Dist Y";
        public string paramGrounded = "Grounded";
        public string paramJump = "Jump";
    }

    // 필드 & 프로퍼티
    [SerializeField] private Components _compo = new Components();
    [SerializeField] private CameraOption _camOption = new CameraOption();
    [SerializeField] private KeyOption _key = new KeyOption();
    [SerializeField] private StateOption _state = new StateOption();
    [SerializeField] private AnimatorOption _animOption = new AnimatorOption();

    public Components Compo => _compo;
    public CameraOption CamOption => _camOption;
    public KeyOption Key => _key;
    public StateOption State => _state;
    public AnimatorOption AnimOption => _animOption;

    protected float castRadius; // 캡슐 캐스트 반지름값
    protected float capsuleRadiusDiff; // 판정 보정치

    protected Vector2 rotation; // 마우스 움직임을 통해 얻는 회전 값
    protected float camInitDist; // 초기 거리 값
    protected float zoomDistance; // 현재 줌 거리
    protected float camDistance; // 실제 카메라 거리

    protected virtual void InitializeComponent()
    {
        InitRigidBody();
        InitCapsuleCollider();
        InitCamComponent();
        Compo.anim = GetComponentInChildren<Animator>();
    }

    private void InitCamComponent()
    {
        // 카메라 구성 오브젝트 바인딩
        Compo.camObj = Compo.cam.gameObject;
        Compo.camRig = Compo.cam.transform.parent;
        Compo.camRoot = Compo.camRig.parent;
    }

    // 리지드바디 컴포넌트 설정
    private void InitRigidBody()
    {
        TryGetComponent(out Compo.rigd);
        if (Compo.rigd == null) Compo.rigd = gameObject.AddComponent<Rigidbody>();

        Compo.rigd.constraints = RigidbodyConstraints.FreezeRotation; // 물리 회전 제한
        Compo.rigd.interpolation = RigidbodyInterpolation.Interpolate; // 현 프레임과 다음 프레임 사이의 랜더링 차이를 선형보간
        Compo.rigd.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌판정 추정 활성화
        Compo.rigd.useGravity = false; // 중력 수동제어를 위한 옵션 비활성화
    }

    // 캡슐 콜라이더 설정
    private void InitCapsuleCollider()
    {
        TryGetComponent(out Compo.capsule);
        if (Compo.capsule == null)
        {
            Compo.capsule = gameObject.AddComponent<CapsuleCollider>();

            float height = -1f; // 콜라이더 높이

            var skinMeshArray = GetComponentsInChildren<SkinnedMeshRenderer>(); // 플레이어 모델의 스킨 메시 랜더러 배열 가져오기
            // 스킨 메시 랜더러를 사용하는 경우
            if (skinMeshArray.Length > 0)
            {
                // 모든 메시 탐색
                foreach (var mesh in skinMeshArray)
                {
                    foreach (var vertex in mesh.sharedMesh.vertices)
                    {
                        if (height < vertex.y)
                        {
                            height = vertex.y;
                        }
                    }
                }
            }
            // 메시 랜더러를 사용하는 경우
            else
            {
                var meshFilterArray = GetComponentsInChildren<MeshFilter>();
                if (meshFilterArray.Length > 0)
                {
                    // 모든 메시 탐색
                    foreach (var mesh in meshFilterArray)
                    {
                        foreach (var vertex in mesh.mesh.vertices)
                        {
                            if (height < vertex.y)
                            {
                                height = vertex.y;
                            }
                        }
                    }
                }
            }

            // 높이값이 0보다 작을 경우 1로 초기화
            if (height <= 0)
            {
                height = 1.0f;
            }

            float center = height * 0.5f; // 중심점 초기화

            Compo.capsule.height = height;
            Compo.capsule.center = Vector3.up * center;
            Compo.capsule.radius = 0.2f;
        }
        castRadius = Compo.capsule.radius * 0.9f;
        capsuleRadiusDiff = Compo.capsule.radius - castRadius + 0.05f;
    }
}
