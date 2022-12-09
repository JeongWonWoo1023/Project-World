using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController
{
    private float deltaTime;

    private float wheelInput = 0; // 줌 기능 휠 입력 값
    private float lerpWheel; // 보간 된 휠 입력 값
    private float zoomDelta;

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

        Current.camDistance = Current.zoomDistance = Current.camInitDist = Vector3.Distance(Compo.rig.position, Compo.cam.transform.position);
    }

    // 카메라가 항상 지면 위에 위치하게 설정
    private void SetOnTheGround()
    {
        Transform root = Compo.root, cam = Compo.camObj.transform;

        State.isObstacle = Physics.Raycast(root.position, -cam.forward, out var hit, Current.zoomDistance + 2.0f, CamOption.groundMask);
        if (State.isObstacle)
        {
            // 현재 카메라의 거리 캐싱
            Current.camDistance = Mathf.Clamp(Vector3.Distance(root.position, cam.position),
                            Current.camInitDist - CamOption.zoomInDistance,
                            Current.camInitDist + CamOption.zoomOutDistance);
            if (Current.camDistance > Current.camInitDist - CamOption.zoomInDistance)
            {
                cam.position = hit.point + cam.forward * 1.0f;
            }
        }
        else if(Current.camDistance < Current.zoomDistance)
        {
            float lerp;
            if (Current.camDistance < CamOption.zoomInDistance)
            {
                lerp = Mathf.Lerp(Current.camDistance, Current.zoomDistance, 0.5f);
            }
            else
            {
                lerp = Mathf.Lerp(Current.camDistance, Current.zoomDistance, 0.001f);
            }
            Vector3 move = Vector3.back * lerp * deltaTime;
            cam.Translate(move, Space.Self);
            Current.camDistance = Vector3.Distance(root.position, cam.position);
        }

    }

    // 카메라 관련 입력 처리
    private void SetCameraInput()
    {
        Current.rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        wheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        lerpWheel = Mathf.Lerp(lerpWheel, wheelInput, CamOption.zoomAccel); // 부드러운 이동을 위해 보간
    }

    private void Rotate()
    {
        Transform root = Compo.root, rig = Compo.rig;

        RotateModelRoot();

        // 회전속도 계수 연산
        float deltaCoef = deltaTime * 50.0f;

        // X축 회전값 연산 ( 상하 )
        float currentX = rig.localEulerAngles.x;
        float nextX = currentX + Current.rotation.y * CamOption.rotateSpeed * deltaCoef;
        if (nextX > 180.0f)
        {
            nextX -= 360.0f;
        }
        // Y축 회전값 연산 ( 좌우 )
        float currentY = root.localEulerAngles.y;
        float nextY = currentY + Current.rotation.x * CamOption.rotateSpeed * deltaCoef;

        bool rotatableX = CamOption.lookUpLimitAngle < nextX &&
            CamOption.lookDownLimitAngle > nextX;

        // 각 축 회전 적용
        rig.localEulerAngles = Vector3.right * (rotatableX ? nextX : currentX);
        root.localEulerAngles = Vector3.up * nextY;
    }

    private void RotateModelRoot()
    {
        if (Compo.player.State.isMoving == false) return;

        Vector3 dir = Compo.rig.TransformDirection(Compo.player.localMoveDirection);
        float currentY = Compo.modelRoot.localEulerAngles.y;
        float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

        if (nextY - currentY > 180f) nextY -= 360f;
        else if (currentY - nextY > 180f) nextY += 360f;

        Compo.modelRoot.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.1f);
    }

    private void Zoom()
    {
        if (Mathf.Abs(lerpWheel) < 0.01f) return; // 휠 입력이 없을 경우 예외 처리

        Transform cam = Compo.cam.transform;
        Transform rig = Compo.rig;

        float zoomValue = deltaTime * CamOption.zoomSpeed; // 줌 적용 값
        Vector3 move = Vector3.forward * zoomValue * lerpWheel * 10.0f; // 실제 이동 벡터

        Current.zoomDistance = Mathf.Clamp(Current.camInitDist + zoomDelta,
                        Current.camInitDist - CamOption.zoomInDistance,
                        Current.camInitDist + CamOption.zoomOutDistance);

        if (State.isObstacle)
        {
            if (lerpWheel > 0.01f)
            {
                if (Current.camDistance > Current.camInitDist - CamOption.zoomInDistance)
                {
                    cam.Translate(move, Space.Self);
                }
            }
            else if (lerpWheel < -0.01f)
            {
                if (Current.camDistance < Current.camInitDist + CamOption.zoomOutDistance)
                {
                    cam.Translate(move, Space.Self);
                }
            }
        }
        else
        {
            if (Current.camDistance < Current.zoomDistance)
            {
                // 줌인
                if (lerpWheel > 0.01f)
                {
                    // 초기 거리 - 현재 거리 값이 줌인 한계치보다 낮을 경우
                    if (Current.camInitDist - Current.zoomDistance < CamOption.zoomInDistance)
                    {
                        zoomDelta -= move.magnitude;
                    }
                }
                // 줌아웃
                else if (lerpWheel < -0.01f)
                {
                    // 현재 거리 - 초기 거리 값이 줌아웃 한계치보다 낮을 경우
                    if (Current.zoomDistance - Current.camInitDist < CamOption.zoomOutDistance)
                    {
                        zoomDelta += move.magnitude;
                    }
                }
            }
            else
            {
                Current.camDistance = Current.zoomDistance;

                // 줌인
                if (lerpWheel > 0.01f)
                {
                    // 초기 거리 - 현재 거리 값이 줌인 한계치보다 낮을 경우
                    if (Current.camInitDist - Current.zoomDistance < CamOption.zoomInDistance)
                    {
                        cam.Translate(move, Space.Self);
                        zoomDelta -= move.magnitude;
                    }
                }
                // 줌아웃
                else if (lerpWheel < -0.01f)
                {
                    // 현재 거리 - 초기 거리 값이 줌아웃 한계치보다 낮을 경우
                    if (Current.zoomDistance - Current.camInitDist < CamOption.zoomOutDistance)
                    {
                        cam.Translate(move, Space.Self);
                        zoomDelta += move.magnitude;
                    }
                }
            }
        }
    }
}
