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

        public Camera cam;
        public Transform modelRoot;
        [HideInInspector] public Transform camRoot;
        [HideInInspector] public Transform camRig;
        [HideInInspector] public GameObject camObj;
        [HideInInspector] public BattleSystem battleSystem;
        [HideInInspector] public AnimationEvent animEvent;

        [HideInInspector] public IPlayerMovement movement;
        [HideInInspector] public IBattle interBattle;
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
    public class PlayerState
    {
        public bool isMoving; // 걷는 중
        public bool isRunning; // 뛰는 중
        public bool isGrounded; // 지면 접지
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
        public string paramAttack = "Attack";
    }

    // 필드 & 프로퍼티
    [SerializeField] private Components _compo = new Components();
    [SerializeField] private CameraOption _camOption = new CameraOption();
    [SerializeField] private KeyOption _key = new KeyOption();
    [SerializeField] private PlayerState _state = new PlayerState();
    [SerializeField] private AnimatorOption _animOption = new AnimatorOption();

    public Components Compo => _compo;
    public CameraOption CamOption => _camOption;
    public KeyOption Key => _key;
    public PlayerState State => _state;
    public AnimatorOption AnimOption => _animOption;

    protected Vector2 rotation; // 마우스 움직임을 통해 얻는 회전 값
    protected float camInitDist; // 초기 거리 값
    [SerializeField] protected float zoomDistance; // 현재 줌 거리
    [SerializeField] protected float camDistance; // 실제 카메라 거리
    [SerializeField] protected float obstacleDistance; // 카메라 방향의 장애물과의 거리

    protected float deltaTime; // Time.deltaTime 캐싱

    protected virtual void InitializeComponent()
    {
        // 카메라 구성 오브젝트 바인딩
        Compo.camObj = Compo.cam.gameObject;
        Compo.camRig = Compo.cam.transform.parent;
        Compo.camRoot = Compo.camRig.parent;

        InitSetCam();
        Compo.anim = GetComponentInChildren<Animator>();
        Compo.anim.TryGetComponent(out Compo.animEvent);
        TryGetComponent(out Compo.movement);
        TryGetComponent(out Compo.battleSystem);
        TryGetComponent(out Compo.interBattle);
    }

    private void InitSetCam()
    {
        // 모든 카메라 비활성화
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera cam in cams)
        {
            cam.gameObject.SetActive(false);
        }

        // 카메라의 종류가 많아지면 분리 실행 필요
        Compo.camObj.SetActive(true);

        camDistance = zoomDistance = camInitDist = Vector3.Distance(Compo.camRig.position, Compo.cam.transform.position);
    }
}
