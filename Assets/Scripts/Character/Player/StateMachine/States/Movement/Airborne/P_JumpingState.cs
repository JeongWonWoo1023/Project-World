using UnityEngine;
using UnityEngine.InputSystem;

public class P_JumpingState : P_AirborneState
{
    #region 필드 & 프로퍼티
    private P_JumpData jumpdata;
    private bool keepRotating;
    private bool canStartFalling;
    #endregion

    #region 생성자
    public P_JumpingState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        jumpdata = airborneData.JumpData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        stateMachine.Current.MovementSpeedCoef = 0.0f;

        base.Enter();

        stateMachine.Current.MovementDecelerationForce = jumpdata.DecelerationForce;

        stateMachine.Current.RotationData = airborneData.JumpData.RotationData;

        keepRotating = stateMachine.Current.MovementInput != Vector2.zero;

        Jump();
    }

    public override void Exit()
    {
        base.Exit();
        SetRotationData();

        canStartFalling = false;
    }

    public override void Process()
    {
        base.Process();

        if (!canStartFalling && movementUtil.IsMovingUp(stateMachine.Player.Rigid.velocity,0.0f))
        {
            canStartFalling = true;
        }

        if (!canStartFalling || movementUtil.GetVerticalVelocity(stateMachine.Player.Rigid.velocity).y > 0)
        {
            return;
        }

        stateMachine.ChangeState(stateMachine.Fall);
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        if (keepRotating)
        {
            RotateToTargetRotation();
        }

        if (movementUtil.IsMovingUp(stateMachine.Player.Rigid.velocity))
        {
            DecelerateVertically();
        }
    }
    #endregion

    #region 메인 메소드
    private void Jump()
    {
        Vector3 jumpForce = stateMachine.Current.JumpForce;
        Vector3 JumpDirection = stateMachine.Player.transform.forward;
        if (keepRotating)
        {
            UpdateTargetRotation(GetInputDirection());
            JumpDirection = movementUtil.GetTargetRotation(stateMachine.Current.CurrentTargetRotation.y);
        }

        jumpForce.x *= JumpDirection.x;
        jumpForce.z *= JumpDirection.z;

        jumpForce = GetJumpForceOnSlope(jumpForce);

        ResetVelocity();

        stateMachine.Player.Rigid.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
    {
        Vector3 capsuleColliderCenterInWorld = stateMachine.Player.ColliderUtil.ColliderData.Collider.bounds.center;

        Ray downRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorld, Vector3.down);

        if (Physics.Raycast(downRayFromCapsuleCenter, out RaycastHit hit, jumpdata.JumpToGroundRayDistance, stateMachine.Player.LayerData.GroundMask, QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downRayFromCapsuleCenter.direction);

            if (movementUtil.IsMovingUp(stateMachine.Player.Rigid.velocity))
            {
                float forceCoef = jumpdata.OnSlopeUpCoef.Evaluate(groundAngle);

                jumpForce.x *= forceCoef;
                jumpForce.z *= forceCoef;
            }
            if (movementUtil.IsMovingDown(stateMachine.Player.Rigid.velocity))
            {
                float forceCoef = jumpdata.OnSlopeDownCoef.Evaluate(groundAngle);

                jumpForce.y *= forceCoef;
            }
        }

        return jumpForce;
    }
    #endregion

    #region 재사용 메소드
    protected override void ResetSprintState() { }
    #endregion

    #region 입력 메소드
    protected override void OnMovementCanceled(InputAction.CallbackContext context) { }
    #endregion
}
