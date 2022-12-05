using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer : Jeong Won Woo
// Create : 2022. 12. 05.
// Update : 2022. 12. 05.

public class PlayerMovement : PlayerRequire
{
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

    [SerializeField] private SensorOption _sensor = new SensorOption();
    [SerializeField] private MovementOption _moveOption = new MovementOption();
    [SerializeField] private CurrentMovement _current = new CurrentMovement();

    public SensorOption Sensor => _sensor;
    public MovementOption MoveOption => _moveOption;
    public CurrentMovement Current => _current;

    private Vector3 capsuleTop =>
        new Vector3(transform.position.x, transform.position.y + Compo.capsule.height - Compo.capsule.radius, transform.position.z);
    private Vector3 capsuleBottom =>
        new Vector3(transform.position.x, transform.position.y + Compo.capsule.radius, transform.position.z);

    private float fixedDelta;

    protected virtual void Start()
    {
        InitializeComponent();
    }

    protected virtual void FixedUpdate()
    {
        fixedDelta = Time.fixedDeltaTime;

        GroundSensor();
        ForwardSensor();

        SetPhysics();
        SetValues();
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
}
