using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerRequire
{

    public Vector3 localMoveDirection;
    public Vector3 worldMoveDirection;

    private float groundDistance;

    private float moveX = 0.0f;
    private float moveZ = 0.0f;

    private float wheelInput = 0; // 줌 기능 휠 입력 값
    private float lerpWheel; // 보간 된 휠 입력 값
    private float obstacleLerp;

    private void Start()
    {
        InitializeComponent();
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;

        SetKeyInputValue();

        CamObstacleProcess();
        Rotate();
        Zoom();

        SendGroundDistParam();
        SetAnimationParams();
    }

    private void SetAnimationParams()
    {
        float x = localMoveDirection.x, z = localMoveDirection.sqrMagnitude > 0.0f ? 1.0f : 0.0f;
        if (State.isRunning)
        {
            x *= 2.0f;
            z *= 2.0f;
        }

        moveX = Mathf.Lerp(moveX, x, 0.05f);
        moveZ = Mathf.Lerp(moveZ, z, 0.05f);

        Compo.anim.SetFloat(AnimOption.paramMoveX, moveX);
        Compo.anim.SetFloat(AnimOption.paramMoveZ, moveZ);
        Compo.anim.SetFloat(AnimOption.paramDistY, groundDistance);
        Compo.anim.SetBool(AnimOption.paramGrounded, State.isGrounded);
    }

    private void SendGroundDistParam()
    {
        groundDistance = Compo.movement.GetDistanceFormGround();
        State.isGrounded = Compo.movement.IsGrounded();
    }

    // 입력 이벤트 처리
    private void SetKeyInputValue()
    {
        float horizontal = 0.0f, vertical = 0.0f;

        if (Input.GetKey(Key.moveForward)) vertical += 1.0f;
        if (Input.GetKey(Key.moveBackward)) vertical -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) horizontal -= 1.0f;
        if (Input.GetKey(Key.moveRight)) horizontal += 1.0f;
        if(Input.GetMouseButtonDown(0))
        {
            Compo.anim.SetTrigger(AnimOption.paramAttack);
        }

        SendMovementInfo(horizontal, vertical);
        rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        State.isMoving = horizontal != 0 || vertical != 0;
        State.isRunning = Input.GetKey(Key.dash);

        if (Input.GetKeyDown(Key.jump))
        {
            Jump();
        }

        wheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        lerpWheel = Mathf.Lerp(lerpWheel, wheelInput, CamOption.zoomAccel); // 부드러운 이동을 위해 보간
    }

    private void Jump()
    {
        bool jumpSucceeded = Compo.movement.TryJump();

        if (jumpSucceeded)
        {
            // 애니메이션 파트
        }
    }

    private void SendMovementInfo(float horizontal, float vertical)
    {
        localMoveDirection = new Vector3(horizontal, 0.0f, vertical).normalized;
        worldMoveDirection = Compo.camRoot.TransformDirection(localMoveDirection);
        Compo.movement.SetMovement(worldMoveDirection, State.isRunning);
    }

    private void Rotate()
    {
        Transform root = Compo.camRoot, rig = Compo.camRig;

        RotateModelRoot();

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

    private void RotateModelRoot()
    {
        if (State.isMoving == false) return;

        Vector3 dir = Compo.camRig.TransformDirection(localMoveDirection);
        float currentY = Compo.modelRoot.localEulerAngles.y;
        float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

        if (nextY - currentY > 180f) nextY -= 360f;
        else if (currentY - nextY > 180f) nextY += 360f;

        Compo.modelRoot.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.1f);
    }

    private void Zoom()
    {
        if (Mathf.Abs(wheelInput) < 0.01f)
        {
            return; // 휠 입력이 없을 경우 예외 처리
        }

        Transform cam = Compo.cam.transform, root = Compo.camRoot;

        float zoomValue = CamOption.zoomSpeed * deltaTime; // 줌 적용 값
        Vector3 move = Vector3.forward * zoomValue * lerpWheel * 50.0f; // 실제 이동 벡터

        if (State.isObstacle)
        {
            // 줌인
            if (lerpWheel > 0.01f)
            {
                if(camDistance > camInitDist - CamOption.zoomInDistance)
                {
                    zoomDistance -= move.magnitude;
                    cam.Translate(move, Space.Self);
                }
            }
            // 줌아웃
            else if (lerpWheel < -0.01f)
            {
                if(camDistance < obstacleDistance)
                {
                    zoomDistance += move.magnitude;
                    cam.Translate(move, Space.Self);
                }
            }
        }
        else
        {
            // 줌인
            if (lerpWheel > 0.01f)
            {
                if (camDistance > camInitDist - CamOption.zoomInDistance)
                {
                    zoomDistance -= move.magnitude;
                    cam.Translate(move, Space.Self);
                }
            }
            // 줌아웃
            else if (lerpWheel < -0.01f)
            {
                if (camDistance < camInitDist + CamOption.zoomOutDistance)
                {
                    zoomDistance += move.magnitude;
                    cam.Translate(move, Space.Self);
                }
            }
        }
    }

    // 카메라와 캐릭터 사이에 장애물에대한 처리
    private void CamObstacleProcess()
    {
        Transform cam = Compo.cam.transform, root = Compo.camRoot;

        Vector3 origin = root.position + cam.forward;
        float castDist = Vector3.Distance(origin, cam.position);
        State.isObstacle = Physics.SphereCast(origin, CamOption.castRadius, -cam.forward, out RaycastHit hit, castDist + CamOption.castRadius, CamOption.groundMask);
        bool cast = Physics.SphereCast(origin, CamOption.castRadius, -cam.forward, out RaycastHit castHit, castDist + 1.0f, CamOption.groundMask);
        if (State.isObstacle)
        {
            if(camDistance > 0.5f)
            {
                cam.position = hit.point + hit.normal * 0.15f;
            }
        }
        else if(camDistance < zoomDistance && !cast)
        {
            obstacleLerp = Mathf.Lerp(obstacleLerp, (zoomDistance - camDistance) * 10.0f / zoomDistance, 0.1f);
            cam.Translate(Vector3.back * obstacleLerp * deltaTime, Space.Self);
        }
        camDistance = Vector3.Distance(root.position, cam.position);
    }
}
