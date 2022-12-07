using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer : Jeong Won Woo
// Create : 2022. 12. 05.
// Update : 2022. 12. 05.

public abstract class PlayerRequire : MonoBehaviour
{
    // 옵션 클래스
    [Serializable]
    public class Components
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigd;
        [HideInInspector] public CapsuleCollider capsule;
        [HideInInspector] public CameraController playerCam;
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
    }

    // 필드 & 프로퍼티
    [SerializeField] private Components _compo = new Components();
    [SerializeField] private KeyOption _key = new KeyOption();
    [SerializeField] private StateOption _state = new StateOption();

    public Components Compo => _compo;
    public KeyOption Key => _key;
    public StateOption State => _state;

    protected float castRadius; // 캡슐 캐스트 반지름값
    protected float capsuleRadiusDiff; // 판정 보정치

    protected virtual void InitializeComponent()
    {
        InitRigidBody();
        InitCapsuleCollider();
        Compo.anim = GetComponentInChildren<Animator>();
        Compo.playerCam = GetComponentInChildren<CameraController>();
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
