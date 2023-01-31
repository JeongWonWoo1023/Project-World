using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerMovementState
{
    #region 필드 & 프로퍼티
    private SlopeData slopeData;
    #endregion

    #region 생성자
    public PlayerGroundedState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
        slopeData = stateMachine.Player.ColliderUtil.SlopeData;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
        UpdateSprintState();

        UpdateCameraRecenteringState(stateMachine.Current.MovementInput);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
    }

    public override void PhysicalProcess()
    {
        base.PhysicalProcess();

        Float();
    }
    #endregion

    #region 메인 메소드
    // 스프린트 상태 갱신
    private void UpdateSprintState()
    {
        if (stateMachine.Current.IsSprint || stateMachine.Current.MovementInput == Vector2.zero)
        {
            stateMachine.Current.IsSprint = false;
        }
    }

    private bool IsThereGroundUnderneath()
    {
        BoxCollider groundSensorCollider = stateMachine.Player.ColliderUtil.TriggerColliderData.GroundSensorCollider;
        Vector3 groundColliderCenterInWorld = groundSensorCollider.bounds.center;

        Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorld,
            stateMachine.Player.ColliderUtil.TriggerColliderData.GroundSensorColliderExtents,
            groundSensorCollider.transform.rotation,
            stateMachine.Player.LayerData.GroundMask, QueryTriggerInteraction.Ignore);

        return overlappedGroundColliders.Length > 0;

    }

    // 경사로 처리
    private void Float()
    {
        Vector3 worldCenter = stateMachine.Player.ColliderUtil.ColliderData.Collider.bounds.center;
        Ray centerOriginRay = new Ray(worldCenter, Vector3.down);

        bool cast = Physics.Raycast(centerOriginRay, out RaycastHit hit,
            slopeData.RayDistance, stateMachine.Player.LayerData.GroundMask,
            QueryTriggerInteraction.Ignore);

        if (cast)
        {
            float groungAngle = Vector3.Angle(hit.normal, -centerOriginRay.direction);

            float slopeSpeedCoef = SetSlopeSpeedOnAngle(groungAngle);

            if (Mathf.Approximately(slopeSpeedCoef, 0.0f)) return;

            float floatingDistance = stateMachine.Player.ColliderUtil.ColliderData.localCenter.y
                * stateMachine.Player.transform.localScale.y - hit.distance;

            if (Mathf.Approximately(floatingDistance, 0.0f)) return;

            float amountToLift = floatingDistance * slopeData.StepReachForce - movementUtil.GetVerticalVelocity(stateMachine.Player.Rigid.velocity).y;
            Vector3 liftForce = new Vector3(0.0f, amountToLift, 0.0f);
            stateMachine.Player.Rigid.AddForce(liftForce, ForceMode.VelocityChange);
        }
        else if(!stateMachine.Player.Rigid.useGravity)
        {
            stateMachine.Player.Rigid.useGravity = true;
        }

    }

    // 경사로 각도에 따라 이동속도 보정
    private float SetSlopeSpeedOnAngle(float angle)
    {
        float slopeSpeedCoef = groundedData.SlopeSpeedCoef.Evaluate(angle);

        if (stateMachine.Current.MovementOnSlopeSpeed != slopeSpeedCoef)
        {
            stateMachine.Current.MovementOnSlopeSpeed = slopeSpeedCoef;

            UpdateCameraRecenteringState(stateMachine.Current.MovementInput);
        }

        return slopeSpeedCoef;
    }
    #endregion

    #region 재사용 메소드
    protected override void AddInputAction()
    {
        base.AddInputAction();

        stateMachine.Player.Input.InGameActions.Dash.started += OnDashStarted;

        stateMachine.Player.Input.InGameActions.Jump.started += OnJumpStared;

        stateMachine.Player.Input.InGameActions.NormalAttack.performed += OnNormalAtackPerformed;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();

        stateMachine.Player.Input.InGameActions.Dash.started -= OnDashStarted;

        stateMachine.Player.Input.InGameActions.Jump.started -= OnJumpStared;

        stateMachine.Player.Input.InGameActions.NormalAttack.performed -= OnNormalAtackPerformed;
    }

    protected override void OnContactGroundExited(Collider collider)
    {
        base.OnContactGroundExited(collider);
        if (IsThereGroundUnderneath())
        {
            return;
        }

        Vector3 capsuleColliderCenterInWorld = stateMachine.Player.ColliderUtil.ColliderData.Collider.bounds.center;

        Ray ray = new Ray(capsuleColliderCenterInWorld - stateMachine.Player.ColliderUtil.ColliderData.verticalExtents, Vector3.down);

        if (!Physics.Raycast(ray, out _, groundedData.GroundSensorRayDistance, stateMachine.Player.LayerData.GroundMask, QueryTriggerInteraction.Ignore))
        {
            OnFall();
        }
    }
    #endregion

    #region 재사용 가상 메소드
    protected virtual void OnMove()
    {
        if (stateMachine.Current.IsSprint)
        {
            stateMachine.ChangeState(stateMachine.Sprint);
            return;
        }
        if (stateMachine.Current.IsWalk)
        {
            stateMachine.ChangeState(stateMachine.Walk);
            return;
        }
        stateMachine.ChangeState(stateMachine.Run);
    }

    protected virtual void OnFall()
    {
        stateMachine.ChangeState(stateMachine.Fall);
    }
    #endregion

    #region 입력 메소드
    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.Dash);
    }

    protected virtual void OnJumpStared(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.Jump);
    }

    protected void OnNormalAtackPerformed(InputAction.CallbackContext context)
    {
        // 기본공격 상태로 전환
        if(UIManager.Instance.IsPause)
        {
            return;
        }
        stateMachine.ChangeState(stateMachine.NormalAttack);
    }
    #endregion
}
