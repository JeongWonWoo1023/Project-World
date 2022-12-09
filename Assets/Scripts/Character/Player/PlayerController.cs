using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerMovement
{

    public Vector3 localMoveDirection;
    public Vector3 worldMoveDirection;

    private float groundDistance;

    private float moveX = 0.0f;
    private float moveZ = 0.0f;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        SetKeyInputValue();
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
        groundDistance = GetDistanceFormGround();
        State.isGrounded = IsGrounded();
    }


    // 입력 이벤트 처리
    private void SetKeyInputValue()
    {
        float horizontal = 0.0f, vertical = 0.0f;

        if (Input.GetKey(Key.moveForward)) vertical += 1.0f;
        if (Input.GetKey(Key.moveBackward)) vertical -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) horizontal -= 1.0f;
        if (Input.GetKey(Key.moveRight)) horizontal += 1.0f;

        SendMovementInfo(horizontal, vertical);
        Compo.playerCam.Current.rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        State.isMoving = horizontal != 0 || vertical != 0;
        State.isRunning = Input.GetKey(Key.dash);

        if (Input.GetKeyDown(Key.jump))
        {
            Jump();
        }
    }

    private void Jump()
    {
        bool jumpSucceeded = TryJump();

        if (jumpSucceeded)
        {
            // 애니메이션 파트
        }
    }

    private void SendMovementInfo(float horizontal, float vertical)
    {
        localMoveDirection = new Vector3(horizontal, 0.0f, vertical).normalized;
        worldMoveDirection = Compo.playerCam.Compo.root.TransformDirection(localMoveDirection);
        SetMovement(worldMoveDirection, State.isRunning);
    }
}
