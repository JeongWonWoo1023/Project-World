using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : CameraRequire
{
    private float deltaTime;

    private Vector2 rotation;
    private float wheelInput = 0; // 줌 기능 휠 입력 값
    private float lerpWheel; // 보간 된 휠 입력 값

    private void Start()
    {
        InitializeComponent();
        InitSetCam();
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;
        SetCameraInput();

        SetOnTheGround();
        Rotate();
        Zoom();
    }

    private void InitSetCam()
    {
        // 모든 카메라 비활성화
        var cams = FindObjectsOfType<Camera>();
        foreach (var cam in cams)
        {
            cam.gameObject.SetActive(false);   
        }

        // 카메라의 종류가 많아지면 분리 실행 필요
        Compo.camObj.SetActive(true);

        Current.camInitDist = Vector3.Distance(Compo.rig.position, Compo.cam.transform.position);
        Current.currentZoomDistance = Current.camInitDist;
    }

    // 카메라가 항상 지면 위에 위치하게 설정
    private void SetOnTheGround()
    {
        Transform root = Compo.root, cam = Compo.camObj.transform;
        float dist = Vector3.Distance(root.localPosition, cam.localPosition);
        bool cast = Physics.Raycast(root.position, -cam.forward, out var hit, Current.currentZoomDistance, CamOption.groundMask);
        State.isObstacle = cast;
        if (cast)
        {
            Current.hitDist = Mathf.Clamp(hit.distance, 0, Current.currentZoomDistance);
            if (dist > Current.hitDist)
            {
                cam.position = hit.point + (cam.forward * 0.5f);
            }
        }
        else if(Current.hitDist < Current.currentZoomDistance)
        {
            float lerp = Mathf.Lerp(Current.hitDist, Current.currentZoomDistance, 0.001f);
            Vector3 move = Vector3.back * lerp * deltaTime;
            if (dist < Current.currentZoomDistance)
            {
                cam.Translate(move, Space.Self);
            }
        }

    }

    // 카메라 관련 입력 처리
    private void SetCameraInput()
    {
        rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        wheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        lerpWheel = Mathf.Lerp(lerpWheel, wheelInput, CamOption.zoomAccel); // 부드러운 이동을 위해 보간
    }

    private void Rotate()
    {
        Transform root = Compo.root, rig = Compo.rig;

        // 회전속도 계수 연산
        float deltaCoef = deltaTime * 50.0f;

        // X축 회전값 연산 ( 상하 )
        float currentX = rig.localEulerAngles.x;
        float nextX = currentX + rotation.y * CamOption.rotateSpeed * deltaCoef;
        if (nextX > 180.0f)
        {
            nextX -= 360.0f;
        }
        // Y축 회전값 연산 ( 좌우 )
        float currentY = root.localEulerAngles.y;
        float nextY = currentY + rotation.x * CamOption.rotateSpeed * deltaCoef;

        bool rotatableX = CamOption.lookUpLimitAngle < nextX &&
            CamOption.lookDownLimitAngle > nextX;

        // 각 축 회전 적용
        rig.localEulerAngles = Vector3.right * (rotatableX ? nextX : currentX);
        root.localEulerAngles = Vector3.up * nextY;
    }

    private void Zoom()
    {
        if (Mathf.Abs(lerpWheel) < 0.01f) return; // 휠 입력이 없을 경우 예외 처리

        Transform camTrans = Compo.cam.transform;
        Transform rig = Compo.rig;

        float zoomValue = deltaTime * CamOption.zoomSpeed; // 줌 적용 값
        if (!State.isObstacle)
        {
            Current.currentZoomDistance = Mathf.Clamp(Vector3.Distance(camTrans.position, rig.position),
                        Current.camInitDist - CamOption.zoomInDistance,
                        Current.camInitDist + CamOption.zoomOutDistance);
        }

        Vector3 move = Vector3.forward * zoomValue * lerpWheel * 10.0f; // 실제 이동 벡터

        // 줌인
        if (lerpWheel > 0.01f)
        {
            // 초기 거리 - 현재 거리 값이 줌인 한계치보다 낮을 경우
            if (Current.camInitDist - Current.currentZoomDistance < CamOption.zoomInDistance)
            {
                camTrans.Translate(move, Space.Self);
            }
        }
        // 줌아웃
        else if (lerpWheel < -0.01f)
        {
            // 현재 거리 - 초기 거리 값이 줌아웃 한계치보다 낮을 경우
            if (Current.currentZoomDistance - Current.camInitDist < CamOption.zoomOutDistance)
            {
                camTrans.Translate(move, Space.Self);
            }
        }
    }
}
