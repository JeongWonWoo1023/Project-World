using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : PlayerStateCore
{
    #region 필드 & 프로퍼티
    protected P_GroundedData groundedData; // 지면 이동 관련 데이터
    protected P_AirborneData airborneData; // 공중 이동 관련 데이터
    #endregion

    #region 생성자
    public PlayerMovementState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        // 데이터 바인딩
        groundedData = stateMachine.Player.Data.Movement.GroundedData;
        airborneData = stateMachine.Player.Data.Movement.AirborneData;

        SetBaseCameraRecenteringData();
        InitializeData();
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
    }

    public override void PhysicalProcess()
    {
        Move();
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (stateMachine.Player.LayerData.IsGrounded(collider.gameObject.layer))
        {
            OnContectGround(collider);

            return;
        }
    }

    public override void OnTriggerExit(Collider collider)
    {
        if (stateMachine.Player.LayerData.IsGrounded(collider.gameObject.layer))
        {
            OnContactGroundExited(collider);

            return;
        }
    }
    #endregion

    #region 이니셜라이저 메소드
    private void InitializeData()
    {
        SetRotationData();
    }
    #endregion

    #region 메인 메소드

    // 이동
    private void Move() 
    {
        // 아무 키 입력이 없거나 이동속도 계수가 0일 경우 혹은 공격중일 경우 리턴
        if (stateMachine.Current.MovementInput == Vector2.zero || stateMachine.Current.MovementSpeedCoef == 0.0f)
        {
            return;
        }

        Vector3 direction = GetInputDirection();
        float rotateValue = Rotate(direction); // 이동 방향을 전방으로 캐릭터 회전

        Vector3 targetDirection = movementUtil.GetTargetRotation(rotateValue); // 목표 방향
        float speed = GetMovementSpeed(); //이동속도

        Vector3 velocity = movementUtil.GetHorizontalVelocity(stateMachine.Player.Rigid.velocity); // 물리적 가속도

        stateMachine.Player.Rigid.AddForce(targetDirection * speed - velocity, ForceMode.VelocityChange);
    }

    protected float Rotate(Vector3 dir)
    {
        float angle = UpdateTargetRotation(dir);
        RotateToTargetRotation();
        return angle;
    }
    #endregion

    #region 재사용 메소드
    // 방향값 설정
    protected void SetRotationData()
    {
        stateMachine.Current.RotationData = groundedData.RotationData;

        stateMachine.Current.TimeToReachTargetRotation = stateMachine.Current.RotationData.ReachTargetRotation;
    }

    // 카메라 보정값 설정
    protected void SetBaseCameraRecenteringData()
    {
        stateMachine.Current.BackwardCameraRecenteringData = groundedData.BackwardCameraRecenteringData;
        stateMachine.Current.SidewayCameraRecenteringData = groundedData.SidewayCameraRecenteringData;
    }

    // 이동속도 반환
    protected float GetMovementSpeed(bool isConsiderSlope = true)
    {
        float movementSpeed = groundedData.BaseMovementSpeed * stateMachine.Current.MovementSpeedCoef;

        if (isConsiderSlope)
        {
            movementSpeed *= stateMachine.Current.MovementOnSlopeSpeed;
        }
        return movementSpeed;
    }

    // 목표 방향으로 회전
    protected void RotateToTargetRotation()
    {
        float currentY = stateMachine.Player.Rigid.rotation.eulerAngles.y; // 현재 y축 회전값

        if (currentY == stateMachine.Current.CurrentTargetRotation.y)
        {
            return;
        }
        float smoothY = Mathf.SmoothDampAngle(currentY,
            stateMachine.Current.CurrentTargetRotation.y, ref stateMachine.Current.DampedTargetRotationCurrentValocity.y,
            stateMachine.Current.TimeToReachTargetRotation.y - stateMachine.Current.DampedTargetRotationPassedTime.y);

        stateMachine.Current.DampedTargetRotationPassedTime.y += Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0.0f, smoothY, 0.0f);
        stateMachine.Player.Rigid.MoveRotation(targetRotation);
    }

    // 카메라 보정이동 갱신
    protected void UpdateCameraRecenteringState(Vector2 movementInput)
    {
        if (movementInput == Vector2.zero)
        {
            return;
        }

        if (movementInput == Vector2.up)
        {
            DisableCameraRecentering();
            return;
        }

        float cameraVerticalAngle = stateMachine.Player.MainCameraTrans.eulerAngles.x;

        if (cameraVerticalAngle >= 270.0f)
        {
            cameraVerticalAngle -= 360.0f;
        }

        cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);

        if (movementInput == Vector2.down)
        {
            SetCametaRecenteringState(cameraVerticalAngle, stateMachine.Current.BackwardCameraRecenteringData);

            return;
        }

        SetCametaRecenteringState(cameraVerticalAngle, stateMachine.Current.SidewayCameraRecenteringData);
    }

    // 카메라 보정이동 비활성화
    protected void DisableCameraRecentering()
    {
        stateMachine.Player.CameraUtil.DisableRecentering();
    }

    // 카메라 보정이동 활성화
    protected void EnableCameraRecentering(float waitTime = -1.0f, float recenteringTime = -1.0f)
    {
        float movementSpeed = GetMovementSpeed();
        if (Mathf.Approximately(movementSpeed, 0.0f))
        {
            movementSpeed = groundedData.BaseMovementSpeed;
        }
        stateMachine.Player.CameraUtil.EnableRecentering(waitTime, recenteringTime, groundedData.BaseMovementSpeed, movementSpeed);
    }

    // 카메라 보정이동 설정
    protected void SetCametaRecenteringState(float cameraVerticalAngle, List<PlayerCameraRecenteringData> cameraRecenteringData)
    {
        foreach (PlayerCameraRecenteringData recenteringData in cameraRecenteringData)
        {
            if (!recenteringData.IsWithinRange(cameraVerticalAngle))
            {
                continue;
            }

            EnableCameraRecentering(recenteringData.WaitTime, recenteringData.RecenteringTime);

            return;
        }

        DisableCameraRecentering();
    }

    // 가속도 초기화
    protected void ResetVelocity()
    {
        stateMachine.Player.Rigid.velocity = Vector3.zero;
    }

    // 수직 가속도 초기화
    protected void ResetVerticalVelocity()
    {
        Vector3 playerHorizontalVelocity = movementUtil.GetHorizontalVelocity(stateMachine.Player.Rigid.velocity);

        stateMachine.Player.Rigid.velocity = playerHorizontalVelocity;
    }

    // 수평이동 감속
    protected void DecelerateHorizontally()
    {
        Vector3 playerHorizontalVelocity = movementUtil.GetHorizontalVelocity(stateMachine.Player.Rigid.velocity);

        stateMachine.Player.Rigid.AddForce(-playerHorizontalVelocity * stateMachine.Current.MovementDecelerationForce, ForceMode.Acceleration);
    }

    // 이동 감속
    protected void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = movementUtil.GetVerticalVelocity(stateMachine.Player.Rigid.velocity);

        stateMachine.Player.Rigid.AddForce(-playerVerticalVelocity * stateMachine.Current.MovementDecelerationForce, ForceMode.Acceleration);
    }
    #endregion

    #region 재사용 가상 메소드
    // 입력 콜백 등록
    protected override void AddInputAction()
    {
        base.AddInputAction();
        stateMachine.Player.Input.InGameActions.ToggleWalk.started += OnWalkToggle;

        stateMachine.Player.Input.InGameActions.Look.started += OnMouseMovementStarted;

        stateMachine.Player.Input.InGameActions.Movement.performed += OnMovementPerformed;

        stateMachine.Player.Input.InGameActions.Movement.canceled += OnMovementCanceled;
    }

    // 입력 콜백 제거
    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
        stateMachine.Player.Input.InGameActions.ToggleWalk.started -= OnWalkToggle;

        stateMachine.Player.Input.InGameActions.Look.started -= OnMouseMovementStarted;

        stateMachine.Player.Input.InGameActions.Movement.performed -= OnMovementPerformed;

        stateMachine.Player.Input.InGameActions.Movement.canceled -= OnMovementCanceled;
    }

    protected virtual void OnContectGround(Collider collider)
    {

    }

    protected virtual void OnContactGroundExited(Collider collider)
    {

    }
    #endregion

    #region 입력 메소드
    // 걷기모드 전환
    protected virtual void OnWalkToggle(InputAction.CallbackContext context)
    {
        stateMachine.Current.IsWalk = !stateMachine.Current.IsWalk;
    }

    protected void OnMouseMovementStarted(InputAction.CallbackContext context)
    {
        UpdateCameraRecenteringState(stateMachine.Current.MovementInput);
    }

    protected override void OnMovementPerformed(InputAction.CallbackContext context)
    {
        base.OnMovementPerformed(context);
        UpdateCameraRecenteringState(context.ReadValue<Vector2>());
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        DisableCameraRecentering();
    }
    #endregion
}
