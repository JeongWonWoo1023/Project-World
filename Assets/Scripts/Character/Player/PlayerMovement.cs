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
        [Tooltip("�������� ������ ���̾�")]
        public LayerMask groundMask = -1;

        [Range(0.01f, 0.5f), Tooltip("���� ���� ���� �Ÿ�")]
        public float forwardSensorDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("���� ���� ���� �Ÿ�")]
        public float groundSensorDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("�������� ���� �� ���ġ")]
        public float groundThreshold = 0.01f;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1.0f, 10.0f), Tooltip("�̵��ӵ�")]
        public float moveSpeed = 5.0f;

        [Range(1.0f, 3.0f), Tooltip("�޸��� ���� ���")]
        public float dashCoef = 1.5f;

        [Range(1.0f, 3.0f), Tooltip("�ȱ��� ���� ���")]
        public float walkCoef = 1.5f;

        [Range(1.0f, 10.0f), Tooltip("���� ����")]
        public float jumpForce = 5.0f;

        [Range(0.0f, 2.0f), Tooltip("���� ��Ÿ��")]
        public float jumpCoolTime = 0.5f;

        [Range(0, 3), Tooltip("���� ��� Ƚ��")]
        public int maxJumpCount = 1;

        [Range(1.0f, 75.0f), Tooltip("�ɾ �ö� �� �ִ� �ִ� ��簢")]
        public float maxSlopeAngle = 50.0f;

        [Range(0.0f, 4.0f), Tooltip("���� �̵� ���ӵ�")]
        public float slopeAccel = 1.0f;

        [Range(-9.81f, 0.0f), Tooltip("�߷� ��")]
        public float gravity = -9.81f;
    }

    [Serializable]
    public class CurrentMovement
    {
        [Header("���� �̵� ��")]
        public Vector3 worldDirection;
        public Vector3 groundNormal;
        public Vector3 groundCross;
        public Vector3 horizontalVelocity;

        [Space, Header("���� ���� ��")]
        public float jumpCoolTime;
        public int jumpCount;
        public float outOfControlDuration;

        [Space, Header("���� ������ ���� ����")]
        public float groundDistance;
        public float slopeAngle; // ���� �ٴ� ���
        public float verticalSlopeAngle; // ���� �翬�� ��
        public float forwardSlopeAngle; // �ٶ󺸴� ������ ��簢
        public float slopeAccel; // ���� ���ӵ�

        [Space, Header("���� �߷� ��")]
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

    // �̵� �Ҵ� ���� ����
    public void SetOutOfControl(float time)
    {
        Current.outOfControlDuration = time;
        ResetJump();
    }

    // ���� �ʱ�ȭ
    private void ResetJump()
    {
        Current.jumpCoolTime = 0.0f;
        Current.jumpCount = 0;
        State.isJumping = false;
        State.isJumpTrig = false;
    }

    // �ϴ� ���� ����
    private void GroundSensor()
    {
        Current.groundDistance = float.MaxValue;
        Current.groundNormal = Vector3.up;
        Current.slopeAngle = 0.0f;
        Current.forwardSlopeAngle = 0.0f;

        // �ϴܿ� ��ü ���� �߻�
        bool cast = 
            Physics.SphereCast(capsuleBottom, castRadius, Vector3.down, out var hit, Sensor.groundSensorDistance, Sensor.groundMask, QueryTriggerInteraction.Ignore);

        State.isGrounded = false; // ���� ���� ���� �ʱ�ȭ

        // ���� ���� ������ ��
        if (cast)
        {
            // ���� �������� �ʱ�ȭ
            Current.groundNormal = hit.normal;

            // ���� ��簢 ���
            Current.slopeAngle = Vector3.Angle(Current.groundNormal, Vector3.up);
            Current.forwardSlopeAngle = Vector3.Angle(Current.groundNormal, Current.worldDirection) - 90.0f;

            // �̵����� ��簢 üũ
            State.isUnableMoveSlope = Current.slopeAngle >= MoveOption.maxSlopeAngle;

            // ������� �Ÿ� ���
            Current.groundDistance = Mathf.Max(hit.distance - capsuleRadiusDiff - Sensor.groundThreshold, 0.0f);
            
            // ���� ���� ���� üũ
            State.isGrounded =
                (Current.groundDistance <= 0.0001f) && !State.isUnableMoveSlope;

        }
        // �������Ϳ� ��簢 �������� ����
        Current.groundCross = Vector3.Cross(Current.groundNormal, Vector3.up);
    }

    // ���� ���� ����
    private void ForwardSensor()
    {
        bool cast =
            Physics.CapsuleCast(capsuleBottom, capsuleTop, castRadius, Current.worldDirection + Vector3.down * 0.1f,
            out var hit, Sensor.forwardSensorDistance, -1, QueryTriggerInteraction.Ignore);

        State.isBlocked = false; // ���� ��ֹ� ���� �ʱ�ȭ

        if (cast)
        {
            float forwardObjAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isBlocked = forwardObjAngle >= MoveOption.maxSlopeAngle;
        }
    }

    // ���� ���� ����
    private void SetPhysics()
    {
        if (State.isGrounded)
        {
            Current.gravity = 0.0f; // �߷� ������

            Current.jumpCount = 0;
            State.isJumping = false;
        }
        else
        {
            Current.gravity += fixedDelta * MoveOption.gravity; // �߷� ����
        }
    }

    //�� ����
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
