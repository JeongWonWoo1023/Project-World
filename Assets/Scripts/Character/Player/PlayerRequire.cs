using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer : Jeong Won Woo
// Create : 2022. 12. 05.
// Update : 2022. 12. 05.

public abstract class PlayerRequire : MonoBehaviour
{
    // �ɼ� Ŭ����
    [Serializable]
    public class Components
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigd;
        [HideInInspector] public CapsuleCollider capsule;
    }

    [Serializable]
    public class KeyOption
    {
        // �̵�
        public KeyCode moveForward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;

        public KeyCode jump = KeyCode.Space; // ����
        public KeyCode dash = KeyCode.LeftShift; // �뽬
        public KeyCode walkMod = KeyCode.LeftControl; // �ȱ��� ��ȯ
    }

    [Serializable]
    public class StateOption
    {
        public bool isMoving; // �ȴ� ��
        public bool isRunning; // �ٴ� ��
        public bool isGrounded; // ���� ����
        public bool isUnableMoveSlope; // �ɾ ��� �Ұ����� ���ο� �ִ� ���
        public bool isJumpTrig; // ���� Ʈ����
        public bool isJumping; // ���� ��
        public bool isBlocked; // ���濡 ��ֹ� ����
        public bool isOutOfControl; // ����Ұ� ����
    }

    // �ʵ� & ������Ƽ
    [SerializeField] private Components _compo = new Components();
    [SerializeField] private KeyOption _key = new KeyOption();
    [SerializeField] private StateOption _state = new StateOption();

    public Components Compo => _compo;
    public KeyOption Key => _key;
    public StateOption State => _state;

    protected float castRadius; // ĸ�� ĳ��Ʈ ��������
    protected float capsuleRadiusDiff; // ���� ����ġ

    protected virtual void InitializeComponent()
    {
        InitRigidBody();
        InitCapsuleCollider();
        Compo.anim = GetComponentInChildren<Animator>();
    }

    // ������ٵ� ������Ʈ ����
    private void InitRigidBody()
    {
        TryGetComponent(out Compo.rigd);
        if (Compo.rigd == null) Compo.rigd = gameObject.AddComponent<Rigidbody>();

        Compo.rigd.constraints = RigidbodyConstraints.FreezeRotation; // ���� ȸ�� ����
        Compo.rigd.interpolation = RigidbodyInterpolation.Interpolate; // �� �����Ӱ� ���� ������ ������ ������ ���̸� ��������
        Compo.rigd.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // �浹���� ���� Ȱ��ȭ
        Compo.rigd.useGravity = false; // �߷� ������� ���� �ɼ� ��Ȱ��ȭ
    }

    // ĸ�� �ݶ��̴� ����
    private void InitCapsuleCollider()
    {
        TryGetComponent(out Compo.capsule);
        if (Compo.capsule == null)
        {
            Compo.capsule = gameObject.AddComponent<CapsuleCollider>();

            float height = -1f; // �ݶ��̴� ����

            var skinMeshArray = GetComponentsInChildren<SkinnedMeshRenderer>(); // �÷��̾� ���� ��Ų �޽� ������ �迭 ��������
            // ��Ų �޽� �������� ����ϴ� ���
            if (skinMeshArray.Length > 0)
            {
                // ��� �޽� Ž��
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
            // �޽� �������� ����ϴ� ���
            else
            {
                var meshFilterArray = GetComponentsInChildren<MeshFilter>();
                if (meshFilterArray.Length > 0)
                {
                    // ��� �޽� Ž��
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

            // ���̰��� 0���� ���� ��� 1�� �ʱ�ȭ
            if (height <= 0)
            {
                height = 1.0f;
            }

            float center = height * 0.5f; // �߽��� �ʱ�ȭ

            Compo.capsule.height = height;
            Compo.capsule.center = Vector3.up * center;
            Compo.capsule.radius = 0.2f;
        }
        castRadius = Compo.capsule.radius * 0.9f;
        capsuleRadiusDiff = Compo.capsule.radius - castRadius + 0.05f;
    }
}
