using System;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어의 상태기계 최상위 상태 노드
public class PlayerStateCore : StateCore
{
    #region 필드 & 프로퍼티
    protected PlayerStateMachine stateMachine;
    protected MovementMathUtillity movementUtil; // 이동 연산 유틸리티
    #endregion

    #region 생성자
    public PlayerStateCore(PlayerStateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
        movementUtil = new MovementMathUtillity();
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        AddInputAction();
    }

    public override void Exit()
    {
        RemoveInputAction();
    }

    public override void HandleInput()
    {
        ReadMovementInput();
    }

    public override void Process()
    {
        base.Process();
        if(stateMachine.Player.IsHit)
        {
            stateMachine.Player.IsHit = false;
            stateMachine.ChangeState(stateMachine.Hit);
        }
        if (stateMachine.Player.IsDead)
        {
            stateMachine.Player.IsDead = false;
            stateMachine.ChangeState(stateMachine.Dead);
        }
    }
    #endregion

    // 입력 값 적용
    private void ReadMovementInput()
    {
        stateMachine.Current.MovementInput = stateMachine.Player.Input.InGameActions.Movement.ReadValue<Vector2>();
    }

    #region 재사용 메소드
    // 모션 시작
    protected override void StartAnimation(int hash)
    {
        stateMachine.Player.Anim.SetBool(hash, true);
    }

    // 모션 종료
    protected override void StopAnimation(int hash)
    {
        stateMachine.Player.Anim.SetBool(hash, false);
    }

    // int값에 의한 모션 시작
    protected override void SetAnimationValueFromHash(int hash, int value)
    {
        stateMachine.Player.Anim.SetInteger(hash, value);
    }

    // 이동 방향 반환
    protected Vector3 GetInputDirection()
    {
        return new Vector3(stateMachine.Current.MovementInput.x, 0.0f, stateMachine.Current.MovementInput.y);
    }

    // 각도값 갱신
    protected float UpdateTargetRotation(Vector3 direction, bool isAdd = true)
    {
        float angle = movementUtil.GetTargetAtanAngle(direction); // 이동방향 각도값

        if (isAdd) // 양의 방향으로 회전인 경우
        {
            movementUtil.AddRotationAngle(ref angle, stateMachine.Player.MainCameraTrans.eulerAngles.y);
        }

        // 각도값이 현재 바라보는 방향의 각도와 다를 경우
        if (angle != stateMachine.Current.CurrentTargetRotation.y)
        {
            UpdateRotationData(angle);
        }

        return angle;
    }

    // 목표 각도로 현재 각도값 갱신
    private void UpdateRotationData(float targetAngle)
    {
        stateMachine.Current.CurrentTargetRotation.y = targetAngle;
        stateMachine.Current.DampedTargetRotationPassedTime.y = 0.0f;
    }

    #endregion

    #region 재사용 가상 메소드
    // 입력 콜백 등록
    protected virtual void AddInputAction()
    {
        stateMachine.Player.Input.UIActions.Pause.performed += OnEscapeAction;
        stateMachine.Player.Input.UIActions.Inventory.performed += OnInventoryAction;
        stateMachine.Player.Input.UIActions.Cursor.performed += OnCursorAction;
        stateMachine.Player.Input.InGameActions.Interaction.performed += OnGetItemAction;
    }

    // 입력 콜백 등록 해제
    protected virtual void RemoveInputAction()
    {
        stateMachine.Player.Input.UIActions.Pause.performed -= OnEscapeAction;
        stateMachine.Player.Input.UIActions.Inventory.performed -= OnInventoryAction;
        stateMachine.Player.Input.UIActions.Cursor.performed -= OnCursorAction;
        stateMachine.Player.Input.InGameActions.Interaction.performed -= OnGetItemAction;
    }
    #endregion

    #region 입력 메소드
    private void OnEscapeAction(InputAction.CallbackContext context)
    {
        if(UIManager.Instance.IsTalk)
        {
            return;
        }

        if(UIManager.Instance.PopupStack.Count > 1)
        {
            UIManager.Instance.ClosePopup();
            return;
        }

        if (UIManager.Instance.PopupStack.Count == 1)
        {
            UIManager.Instance.ClosePopup();
            if(!UIManager.Instance.IsPause)
            {
                SetOption(true, true, false, 1);
            }
            return;
        }

        if (UIManager.Instance.IsPause)
        {
            // 일시정지 상태일 경우
            UIManager.Instance.IsPause = false;
            SetOption(true, true, false, 1);
        }
        else
        {
            // 일시정지 상태가 아닌 경우
            UIManager.Instance.IsPause = true;
            SetOption(false, false, true, 0);
        }
    }

    private void SetOption(bool cameraEnabled, bool isIngame, bool cursor, int timeScale)
    {
        stateMachine.Player.CameraInput.enabled = cameraEnabled;
        if(isIngame)
        {
            stateMachine.Player.Input.InGameActions.Enable();
        }
        else
        {
            stateMachine.Player.Input.InGameActions.Disable();
        }
        UIManager.Instance.IsCursor = cursor;
        Time.timeScale = timeScale;
    }

    private void OnInventoryAction(InputAction.CallbackContext context)
    {
        if(UIManager.Instance.IsPause || UIManager.Instance.IsTalk)
        {
            return;
        }
        if(UIManager.Instance.IsOpneInventory)
        {
            for(int i = UIManager.Instance.PopupStack.Count; i > 0; --i)
            {
                UIManager.Instance.ClosePopup();
            }
            stateMachine.Player.CameraInput.enabled = true;
            stateMachine.Player.Input.InGameActions.Enable();
            UIManager.Instance.IsCursor = false;
            Time.timeScale = 1;
        }
        else
        {
            UIManager.Instance.OpneInventory();
            stateMachine.Player.CameraInput.enabled = false;
            stateMachine.Player.Input.InGameActions.Disable();
            UIManager.Instance.IsCursor = true;
            Time.timeScale = 0;
        }
    }

    protected virtual void OnNormalAtackAction(InputAction.CallbackContext context)
    {

    }

    private void OnCursorAction(InputAction.CallbackContext context)
    {
        UIManager.Instance.IsCursor = !UIManager.Instance.IsCursor;
    }

    private void OnGetItemAction(InputAction.CallbackContext context)
    {
        stateMachine.Player.NearDetector.GetItem();
        if(stateMachine.Player.NearDetector.NPCList.Count == 0)
        {
            return;
        }
        if(UIManager.Instance.IsTalk)
        {
            UIManager.Instance.TalkDialog.IsNext = true;
            return;
        }
        stateMachine.Player.NearDetector.NPCList[0]?.Talk();
    }

    protected virtual void OnMovementPerformed(InputAction.CallbackContext context)
    {
        if (UIManager.Instance.IsTalk)
        {
            return;
        }
    }

    protected virtual void OnUltimadom(InputAction.CallbackContext context)
    {
        if(stateMachine.Current.UltimadomCoolTime > 0.0f || stateMachine.Player.CurrentMP < stateMachine.Player.Ultimadom.Cost)
        {
            return;
        }
        stateMachine.Player.Anim.SetTrigger("TrigUltimadom");
        stateMachine.ChangeState(stateMachine.Ultimadom);
    }
    #endregion
}
