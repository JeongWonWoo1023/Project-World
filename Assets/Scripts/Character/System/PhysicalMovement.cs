using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer : Jeong Won Woo
// Create : 2022. 12. 05.
// Update : 2022. 12. 05.

public class PhysicalMovement : MonoBehaviour, IMovement
{
    [Serializable]
    public class Components
    {
        [HideInInspector] public Rigidbody rigd;
        [HideInInspector] public CapsuleCollider capsule;
    }

    [Serializable]
    public class SensorOption
    {
        [Tooltip("지면으로 판정할 레이어")]
        public LayerMask groundMask = -1;

        [Range(0.01f, 0.5f), Tooltip("전방 감지 센서 거리")]
        public float forwardSensorDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("지면 감지 센서 거리")]
        public float groundSensorDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("지면으로 감지 할 허용치")]
        public float groundThreshold = 0.01f;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1.0f, 10.0f), Tooltip("이동속도")]
        public float moveSpeed = 5.0f;

        [Range(1.0f, 3.0f), Tooltip("달리기 가속 계수")]
        public float dashCoef = 1.5f;

        [Range(1.0f, 3.0f), Tooltip("걷기모드 감속 계수")]
        public float walkCoef = 1.5f;

        [Range(1.0f, 10.0f), Tooltip("점프 강도")]
        public float jumpForce = 5.0f;

        [Range(0.0f, 2.0f), Tooltip("점프 쿨타임")]
        public float jumpCoolTime = 0.5f;

        [Range(0, 3), Tooltip("점프 허용 횟수")]
        public int maxJumpCount = 1;

        [Range(1.0f, 75.0f), Tooltip("걸어서 올라갈 수 있는 최대 경사각")]
        public float maxSlopeAngle = 50.0f;

        [Range(0.0f, 4.0f), Tooltip("경사로 이동 가속도")]
        public float slopeAccel = 1.0f;

        [Range(-9.81f, 0.0f), Tooltip("중력 값")]
        public float gravity = -9.81f;
    }

    [Serializable]
    public class CurrentMovement
    {
        [Header("현재 이동 값")]
        public Vector3 worldDirection;
        public Vector3 groundNormal;
        public Vector3 groundCross;
        public Vector3 horizontalVelocity;

        [Space, Header("현재 점프 값")]
        public float jumpCoolTime;
        public int jumpCount;
        public float outOfControlDuration;

        [Space, Header("현재 접지한 지면 정보")]
        public float groundDistance;
        public float slopeAngle; // 현재 바닥 경사
        public float verticalSlopeAngle; // 수직 재연산 값
        public float forwardSlopeAngle; // 바라보는 방향의 경사각
        public float slopeAccel; // 경사로 가속도

        [Space, Header("현재 중력 값")]
        public float gravity;
    }

    [Serializable]
    public class MovementState
    {
        public bool isMoving; // 걷는 중
        public bool isRunning; // 뛰는 중
        public bool isGrounded; // 지면 접지
        public bool isUnableMoveSlope; // 걸어서 등반 불가능한 경사로에 있는 경우
        public bool isJumpTrig; // 점프 트리거
        public bool isJumping; // 점프 중
        public bool isBlocked; // 전방에 장애물 존재
        public bool isOutOfControl; // 제어불가 상태
    }

    [SerializeField] private Components _compo = new Components();
    [SerializeField] private SensorOption _sensor = new SensorOption();
    [SerializeField] private MovementOption _moveOption = new MovementOption();
    [SerializeField] private CurrentMovement _current = new CurrentMovement();
    [SerializeField] private MovementState _state = new MovementState();

    public Components Compo => _compo;
    public SensorOption Sensor => _sensor;
    public MovementOption MoveOption => _moveOption;
    public CurrentMovement Current => _current;
    public MovementState State => _state;

    private Vector3 capsuleTop =>
        new Vector3(transform.position.x, transform.position.y + Compo.capsule.height - Compo.capsule.radius, transform.position.z);
    private Vector3 capsuleBottom =>
        new Vector3(transform.position.x, transform.position.y + Compo.capsule.radius, transform.position.z);

    protected float castRadius; // 캡슐 캐스트 반지름값
    protected float capsuleRadiusDiff; // 판정 보정치

    private float fixedDelta;

    protected virtual void Start()
    {
        InitRigidBody();
        InitCapsuleCollider();
    }

    protected virtual void FixedUpdate()
    {
        fixedDelta = Time.fixedDeltaTime;

        GroundSensor();
        ForwardSensor();

        SetPhysics();
        SetValues();

        CalculateMovement();
        ApplyMovement();
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
                    if (mesh.gameObject.layer == LayerMask.NameToLayer("Weapon"))
                    {
                        Debug.Log("Weapon");
                        continue;
                    }
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
                        if (mesh.gameObject.layer == LayerMask.NameToLayer("Weapon"))
                        {
                            Debug.Log("Weapon");
                            continue;
                        }
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
            Compo.capsule.radius = 0.25f;
        }
        castRadius = Compo.capsule.radius * 0.9f;
        capsuleRadiusDiff = Compo.capsule.radius - castRadius + 0.05f;
    }

    // 이동 불능 상태 세팅
    public void SetOutOfControl(float time)
    {
        Current.outOfControlDuration = time;
        ResetJump();
    }

    // 점프 초기화
    private void ResetJump()
    {
        Current.jumpCoolTime = 0.0f;
        Current.jumpCount = 0;
        State.isJumping = false;
        State.isJumpTrig = false;
    }

    // 하단 감지 센서
    private void GroundSensor()
    {
        Current.groundDistance = float.MaxValue;
        Current.groundNormal = Vector3.up;
        Current.slopeAngle = 0.0f;
        Current.forwardSlopeAngle = 0.0f;

        // 하단에 구체 레이 발사
        bool cast = 
            Physics.SphereCast(capsuleBottom, castRadius, Vector3.down, out var hit, Sensor.groundSensorDistance, Sensor.groundMask, QueryTriggerInteraction.Ignore);

        State.isGrounded = false; // 지면 접지 여부 초기화

        // 지면 접지 상태일 때
        if (cast)
        {
            // 지면 법선벡터 초기화
            Current.groundNormal = hit.normal;

            // 현재 경사각 계산
            Current.slopeAngle = Vector3.Angle(Current.groundNormal, Vector3.up);
            Current.forwardSlopeAngle = Vector3.Angle(Current.groundNormal, Current.worldDirection) - 90.0f;

            // 이동가능 경사각 체크
            State.isUnableMoveSlope = Current.slopeAngle >= MoveOption.maxSlopeAngle;

            // 지면과의 거리 계산
            Current.groundDistance = Mathf.Max(hit.distance - capsuleRadiusDiff - Sensor.groundThreshold, 0.0f);
            
            // 지면 접지 여부 체크
            State.isGrounded =
                (Current.groundDistance <= 0.0001f) && !State.isUnableMoveSlope;

        }
        // 수직벡터와 경사각 법선벡터 외적
        Current.groundCross = Vector3.Cross(Current.groundNormal, Vector3.up);
    }

    // 전방 감지 센서
    private void ForwardSensor()
    {
        bool cast =
            Physics.CapsuleCast(capsuleBottom, capsuleTop, castRadius, Current.worldDirection + Vector3.down * 0.1f,
            out var hit, Sensor.forwardSensorDistance, -1, QueryTriggerInteraction.Ignore);

        State.isBlocked = false; // 전방 장애물 여부 초기화

        if (cast)
        {
            float forwardObjAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isBlocked = forwardObjAngle >= MoveOption.maxSlopeAngle;
        }
    }

    // 물리 현상 세팅
    private void SetPhysics()
    {
        if (State.isGrounded)
        {
            Current.gravity = 0.0f; // 중력 미적용

            Current.jumpCount = 0;
            State.isJumping = false;
        }
        else
        {
            Current.gravity += fixedDelta * MoveOption.gravity; // 중력 적용
        }
    }

    //값 세팅
    private void SetValues()
    {
        if (Current.jumpCoolTime > Mathf.Epsilon)
        {
            Current.jumpCoolTime -= fixedDelta;
        }

        State.isOutOfControl = Current.outOfControlDuration > Mathf.Epsilon;

        if (State.isOutOfControl)
        {
            Current.outOfControlDuration -= fixedDelta;
            Current.worldDirection = Vector3.zero;
        }
    }

    // 이동관련 값 연산
    private void CalculateMovement()
    {
        // 제어불가 상태일 경우
        if (State.isOutOfControl)
        {
            Current.horizontalVelocity = Vector3.zero;
            return;
        }

        // 점프 트리거 활성화 후 점프 사용 가능 상태인 경우
        if (State.isJumpTrig && Current.jumpCoolTime <= 0.0f)
        {
            Current.gravity = MoveOption.jumpForce;

            // 점프 쿨타임 적용, 트리거 초기화
            Current.jumpCoolTime = MoveOption.jumpCoolTime;
            State.isJumpTrig = false;
            State.isJumping = true;

            ++Current.jumpCount;
        }

        // XZ평면 이동속도 연산
        // 전방이 막혀있고 지면 접지상태가 아닌경우 혹은 점프중이면서 지면 접지 상태일 경우
        if (State.isBlocked && !State.isGrounded || State.isJumping && State.isGrounded)
        {
            Current.horizontalVelocity = Vector3.zero;
        }
        // 이동 가능한 경우
        else
        {
            // 이동중이 아닌 경우 0, 달리는 중이 아니면 일반 속도, 모두 맞다면 계수 적용
            float speed = !State.isMoving ? 0.0f :
                !State.isRunning ? MoveOption.moveSpeed :
                MoveOption.moveSpeed * MoveOption.dashCoef;

            // 이동속도 적용
            Current.horizontalVelocity = Current.worldDirection * speed;
        }

        // XZ평면 회전
        // 지면 접지상태이거나 지면에 가까운 경우
        if (State.isGrounded || Current.groundDistance < Sensor.groundSensorDistance && !State.isJumping)
        {
            // 이동중이고 전방이 막히지 않은 경우
            if (State.isMoving && !State.isBlocked)
            {
                // 경사로 가감
                if (MoveOption.slopeAccel > 0.0f)
                {
                    bool isPlus = Current.forwardSlopeAngle >= 0.0f;
                    float absAngle = isPlus ? Current.forwardSlopeAngle : -Current.forwardSlopeAngle;
                    float accel = MoveOption.slopeAccel * absAngle * 0.01111f + 1.0f;
                    Current.slopeAccel = !isPlus ? accel : 1.0f / accel;

                    Current.horizontalVelocity *= Current.slopeAccel;
                }

                // 경사로 이동 벡터 회전
                Current.horizontalVelocity =
                    Quaternion.AngleAxis(-Current.slopeAngle, Current.groundCross) * Current.horizontalVelocity;
            }
        }
    }

    // 최종 값 적용
    private void ApplyMovement()
    {
        // 제어 불가 상태일 경우
        if (State.isOutOfControl)
        {
            Compo.rigd.velocity = new Vector3(Compo.rigd.velocity.x, Current.gravity, Compo.rigd.velocity.z);
            return;
        }

        // 물리 운동량 = 현재 이동 방향 + Y축 중력
        Compo.rigd.velocity = Current.horizontalVelocity + Vector3.up * (Current.gravity);
    }

    public bool IsMoving() => State.isMoving;
    public bool IsGrounded() => State.isGrounded;
    public float GetDistanceFormGround() => Current.groundDistance;

    public void SetMovement(in Vector3 worldDirection, bool isRunning)
    {
        Current.worldDirection = worldDirection;
        State.isMoving = worldDirection.sqrMagnitude > 0.01f;
        State.isRunning = isRunning;
    }

    public bool TryJump()
    {
        // 첫 점프 예외 처리
        if (!State.isGrounded && Current.jumpCount == 0) return false;

        // 점프 쿨타임, 횟수 검사
        if (Current.jumpCoolTime > Mathf.Epsilon) return false;
        if (Current.jumpCount >= MoveOption.maxJumpCount) return false;

        // 이동 불가 경사로에서 점프 불가
        if (State.isUnableMoveSlope) return false;

        State.isJumpTrig = true;
        return true;
    }

    public void StopMove()
    {
        Current.worldDirection = Vector3.zero;
        State.isMoving = false;
        State.isRunning = false;
    }

    public void KnockBack(in Vector3 force, float time)
    {
        SetOutOfControl(time);
        Compo.rigd.AddForce(force, ForceMode.Impulse);
    }
}
