// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using System;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine; // 상태기계 인스턴스
    protected PlayerGroundedData movementData;

    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine;
        movementData = stateMachine.Player.Data.GroundedData;
        InitData();
    }

    private void InitData()
    {
        stateMachine.StateData.TimeToReachTargetRotation = movementData.BaseRotation.TargetRotationReachTime;
    }

    #region Interface Methods
    public virtual void Enter()
    {      
        Debug.Log($"Current State : {GetType().Name}"); // 해당 클래스명 출력

        AddInputActionCallback();
    }

    public virtual void Exit()
    {
        RemoveInputActionCallBack();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }      
           
    public virtual void PhysicalProcess()
    {
        Move();
    }

    public virtual void Process()
    {

    }
    #endregion

    private void ReadMovementInput()
    {
        stateMachine.StateData.MovementInput = stateMachine.Player.Input.Actions.Movement.ReadValue<Vector2>();
    }
    private void Move()
    {
        if (stateMachine.StateData.MovementInput == Vector2.zero || stateMachine.StateData.MoveSpeedCoef == 0.0f) return;
        Vector3 movementDirection = GetInputDirection();
        float targetY = Rotate(movementDirection);

        Vector3 targetDirection = GetTargetRotation(targetY);
        float movementSpeed = GetMovementSpeed();
        

        Vector3 currentValocity = GetVelocity();

        stateMachine.Player.Rigid.AddForce(targetDirection * movementSpeed - currentValocity, ForceMode.VelocityChange);
    }

    private float Rotate(Vector3 direction)
    {
        float angle = UpdateDirectionRotation(direction);
        RotateToTargetRotation();
        return angle;
    }

    private void UpdateRotationData(float targetAngle)
    {
        stateMachine.StateData.CurrentTargetRotation.y = targetAngle;
        stateMachine.StateData.DampedTargetRotationPassedTime.y = 0.0f;
    }

    private float GetDirectionAngle(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (angle < 0.0f)
        {
            angle += 360.0f;
        }

        return angle;
    }
    private float AddCameraRotationToAngle(float angle)
    {
        angle += stateMachine.Player.MainCameraTrans.eulerAngles.y;

        if (angle > 360.0f)
        {
            angle -= 360.0f;
        }

        return angle;
    }

    private Vector3 GetVelocity()
    {
        Vector3 velocity = stateMachine.Player.Rigid.velocity;
        velocity.y = 0.0f;
        return velocity;
    }

    protected float GetMovementSpeed()
    {
        Debug.Log(stateMachine.StateData.MovementOnSlopeSpeed);
        return movementData.BaseMoveSpeed* stateMachine.StateData.MoveSpeedCoef* stateMachine.StateData.MovementOnSlopeSpeed;
    }
    protected Vector3 GetInputDirection() => new Vector3(stateMachine.StateData.MovementInput.x, 0.0f, stateMachine.StateData.MovementInput.y);
    protected Vector3 GetPlayerVerticalVelocity() => new Vector3(0.0f, stateMachine.Player.Rigid.velocity.y, 0.0f);

    protected void RotateToTargetRotation()
    {
        float currentY = stateMachine.Player.Rigid.rotation.eulerAngles.y; // 현재 y축 회전값

        if (currentY == stateMachine.StateData.CurrentTargetRotation.y) return;
        float smoothY = Mathf.SmoothDampAngle(currentY, 
            stateMachine.StateData.CurrentTargetRotation.y, ref stateMachine.StateData.DampedTargetRotationCurrentValocity.y, 
            stateMachine.StateData.TimeToReachTargetRotation.y - stateMachine.StateData.DampedTargetRotationPassedTime.y);

        stateMachine.StateData.DampedTargetRotationPassedTime.y += Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0.0f, smoothY, 0.0f);
        stateMachine.Player.Rigid.MoveRotation(targetRotation);
    }

    protected float UpdateDirectionRotation(Vector3 direction, bool isAdd = true)
    {
        float angle = GetDirectionAngle(direction);

        if(isAdd)
        {
            angle = AddCameraRotationToAngle(angle);
        }

        if (angle != stateMachine.StateData.CurrentTargetRotation.y)
        {
            UpdateRotationData(angle);
        }

        return angle;
    }
    protected Vector3 GetTargetRotation(float angle) => Quaternion.Euler(0.0f,angle, 0.0f) * Vector3.forward;

    protected void ResetVelocity() => stateMachine.Player.Rigid.velocity = Vector3.zero;
    protected virtual void AddInputActionCallback() => stateMachine.Player.Input.Actions.ToggleWalk.started += OnWalkToggle;
    protected virtual void RemoveInputActionCallBack() => stateMachine.Player.Input.Actions.ToggleWalk.started -= OnWalkToggle;
    protected virtual void OnWalkToggle(InputAction.CallbackContext context) => stateMachine.StateData.IsWalk = !stateMachine.StateData.IsWalk;
}

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.StateData.MoveSpeedCoef = 0.0f;
        ResetVelocity();
    }

    public override void Process()
    {
        base.Process();

        if (stateMachine.StateData.MovementInput == Vector2.zero) return;

        OnMove();
    }
}

public class PlayerWalkingState : PlayerMovingState
{
    public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.StateData.MoveSpeedCoef = movementData.WalkData.MoveSpeedCoef;
    }

    protected override void OnWalkToggle(InputAction.CallbackContext context)
    {
        base.OnWalkToggle(context);

        stateMachine.ChangeState(stateMachine.RunningState);
    }
}

public class PlayerRunningState : PlayerMovingState
{
    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.StateData.MoveSpeedCoef = movementData.RunData.MoveSpeedCoef;
    }

    protected override void OnWalkToggle(InputAction.CallbackContext context)
    {
        base.OnWalkToggle(context);

        stateMachine.ChangeState(stateMachine.WalkingState);
    }
}

public class PlayerSprintingState : PlayerMovingState
{
    public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }
}

public class PlayerMovingState : PlayerGroundedState
{
    public PlayerMovingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }
}

public class PlayerGroundedState : PlayerMovementState
{
    private SlopeData slopeData;

    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        slopeData = stateMachine.Player.ColliderUtil.SlopeData;
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        Float();
    }

    private void Float()
    {
        Vector3 worldCenter = stateMachine.Player.ColliderUtil.ColliderData.Collider.bounds.center;
        Ray centerOriginRay = new Ray(worldCenter, Vector3.down);

        bool cast = Physics.Raycast(centerOriginRay, out RaycastHit hit,
            slopeData.RayDIstance, stateMachine.Player.LayerData.GroundMask,
            QueryTriggerInteraction.Ignore);

        if (cast)
        {
            float groungAngle = Vector3.Angle(hit.normal, -centerOriginRay.direction);

            float slopeSpeedCoef = SetSlopeSpeedOnAngle(groungAngle);

            if(Mathf.Approximately(slopeSpeedCoef, 0.0f)) return;

            float floatingDistance = stateMachine.Player.ColliderUtil.ColliderData.localCenter.y
                * stateMachine.Player.transform.localScale.y - hit.distance;

            if(Mathf.Approximately(floatingDistance,0.0f)) return;

            float amountToLift = floatingDistance * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;
            Vector3 liftForce = new Vector3(0.0f,amountToLift,0.0f);
            stateMachine.Player.Rigid.AddForce(liftForce, ForceMode.VelocityChange);
        }

    }

    private float SetSlopeSpeedOnAngle(float angle)
    {
        float slopeSpeedCoef = movementData.SlopeSpeedAngle.Evaluate(angle);
        stateMachine.StateData.MovementOnSlopeSpeed = slopeSpeedCoef;

        return slopeSpeedCoef;
    }

    protected virtual void OnMove()
    {
        if (stateMachine.StateData.IsWalk)
        {
            stateMachine.ChangeState(stateMachine.WalkingState);
            return;
        }
        stateMachine.ChangeState(stateMachine.RunningState);
    }

    protected override void AddInputActionCallback()
    {
        base.AddInputActionCallback();
        stateMachine.Player.Input.Actions.Movement.canceled += OnMovementCanceled;
    }

    protected override void RemoveInputActionCallBack()
    {
        base.RemoveInputActionCallBack();
        stateMachine.Player.Input.Actions.Movement.canceled -= OnMovementCanceled;
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.IdlingState);
    }
}
