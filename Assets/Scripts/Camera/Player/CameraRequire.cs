using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraRequire : MonoBehaviour
{
    public enum CamType
    {
        Player
    }

    [Serializable]
    public class Components
    {
        public Camera cam;

        [HideInInspector] public PlayerController player;
        [HideInInspector] public Transform root;
        [HideInInspector] public Transform rig;
        [HideInInspector] public GameObject camObj;
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

        [Range(0.0f, 3.5f), Tooltip("줌인 최대 거리")]
        public float zoomInDistance = 3.0f;

        [Range(0.0f, 3.5f), Tooltip("줌아웃 최대 거리")]
        public float zoomOutDistance = 3.0f;

        [Range(1.0f, 30.0f), Tooltip("줌 속도")]
        public float zoomSpeed = 20.0f;

        [Range(0.01f, 0.5f), Tooltip("줌 보간 속도")]
        public float zoomAccel = 0.1f;
    }

    [Serializable]
    public class StateOption
    {
        public bool isObstacle; // 카메라 루트와 카메라 사이에 장애물이 감지될 경우
    }

    [Serializable]
    public class CurrentValue
    {
        public float camInitDist; // 초기 거리 값
        public float currentZoomDistance; // 현재 줌 거리
        public float hitDist;
        public Vector3 hitPoint; // 장애물에 걸렸을 때의 지점
    }

    [SerializeField] private Components _compo = new Components();
    [SerializeField] private CameraOption _camOption = new CameraOption();
    [SerializeField] private StateOption _state = new StateOption();
    [SerializeField] private CurrentValue _current = new CurrentValue();

    public Components Compo => _compo;
    public CameraOption CamOption => _camOption;
    public StateOption State => _state;
    public CurrentValue Current => _current;

    protected void InitializeComponent()
    {
        // 카메라 구성 오브젝트 바인딩
        Compo.camObj = Compo.cam.gameObject;
        Compo.rig = Compo.cam.transform.parent;
        Compo.root = Compo.rig.parent;

        // 플레이어 컴포넌트 바인딩
        Compo.player = Compo.root.parent.GetComponent<PlayerController>();
    }
}
