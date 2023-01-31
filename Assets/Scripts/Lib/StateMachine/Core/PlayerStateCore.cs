using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어의 상태기계 최상위 상태 노드
public class PlayerStateCore : StateCore
{
    #region 필드 & 프로퍼티
    protected PlayerStateMachine stateMachine;
    #endregion

    #region 생성자
    public PlayerStateCore(PlayerStateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        AddInputAction();
    }

    public override void Exit()
    {
        RemoveInputAction();
    }

    public override void Process()
    {
        base.Process();
    }
    #endregion

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

    #endregion

    #region 재사용 가상 메소드
    // 입력 콜백 등록
    protected virtual void AddInputAction()
    {
        stateMachine.Player.Input.UIActions.Pause.performed += OnEscapeAction;
        stateMachine.Player.Input.UIActions.Inventory.performed += OnInventoryAction;
        stateMachine.Player.Input.InGameActions.GetItem.performed += OnGetItemAction;
    }

    // 입력 콜백 등록 해제
    protected virtual void RemoveInputAction()
    {
        stateMachine.Player.Input.UIActions.Pause.performed -= OnEscapeAction;
        stateMachine.Player.Input.UIActions.Inventory.performed -= OnInventoryAction;
        stateMachine.Player.Input.InGameActions.GetItem.performed -= OnGetItemAction;
    }
    #endregion

    #region 입력 메소드
    private void OnEscapeAction(InputAction.CallbackContext context)
    {
        if(UIManager.Instance.PopupStack.Count > 1)
        {
            UIManager.Instance.CloaePopup();
            return;
        }

        if (UIManager.Instance.PopupStack.Count == 1)
        {
            UIManager.Instance.CloaePopup();
            stateMachine.Player.CameraInput.enabled = true;
            stateMachine.Player.Input.InGameActions.Enable();
            Time.timeScale = 1;
            return;
        }

        if (UIManager.Instance.IsPause)
        {
            // 일시정지 상태일 경우
            UIManager.Instance.IsPause = false;
            stateMachine.Player.CameraInput.enabled = true;
            stateMachine.Player.Input.InGameActions.Enable();
            Time.timeScale = 1;
        }
        else
        {
            // 일시정지 상태가 아닌 경우
            UIManager.Instance.IsPause = true;
            stateMachine.Player.CameraInput.enabled = false;
            stateMachine.Player.Input.InGameActions.Disable();
            Time.timeScale = 0;
        }
    }

    private void OnInventoryAction(InputAction.CallbackContext context)
    {
        if(UIManager.Instance.IsPause)
        {
            return;
        }
        if(UIManager.Instance.IsOpneInventory)
        {
            for(int i = UIManager.Instance.PopupStack.Count; i > 0; --i)
            {
                UIManager.Instance.CloaePopup();
            }
            stateMachine.Player.CameraInput.enabled = true;
            stateMachine.Player.Input.InGameActions.Enable();
            Time.timeScale = 1;
        }
        else
        {
            UIManager.Instance.OpneInventory();
            stateMachine.Player.CameraInput.enabled = false;
            stateMachine.Player.Input.InGameActions.Disable();
            Time.timeScale = 0;
        }
    }

    private void OnGetItemAction(InputAction.CallbackContext context)
    {
        stateMachine.Player.ItemDetector.GetItem();
    }
    #endregion
}
