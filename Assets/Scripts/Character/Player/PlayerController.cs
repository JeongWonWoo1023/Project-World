using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerMovement
{
    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "MoveX";
        public string paramMoveZ = "MoveZ";
        public string paramDistY = "Dist Y";
        public string paramGrounded = "Grounded";
        public string paramJump = "Jump";
    }

    [SerializeField] private AnimatorOption _animOption = new AnimatorOption();
    public AnimatorOption AnimOption => _animOption;

    public Vector3 localMoveDirection;
    public Vector3 worldMoveDirection;

    private float moveX;
    private float moveZ;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        SetKeyInputValue();
    }

    // 입력 이벤트 처리
    private void SetKeyInputValue()
    {
        float horizontal = 0.0f, vertical = 0.0f;

        if (Input.GetKey(Key.moveForward)) vertical += 1.0f;
        if (Input.GetKey(Key.moveBackward)) vertical -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) horizontal -= 1.0f;
        if (Input.GetKey(Key.moveRight)) horizontal += 1.0f;

        localMoveDirection = new Vector3(horizontal, 0.0f, vertical).normalized;
        // 카메라 설정 입력 필요 ( 월드 이동 벡터 초기화 )
        worldMoveDirection = Compo.playerCam.Compo.root.TransformDirection(localMoveDirection);
        SetMovement(worldMoveDirection, State.isRunning);

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
}
