// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public PlayerAction InputAction { get; private set; } // 인풋 시스템 클래스 프로퍼티
    public PlayerAction.PlayerActions InGameActions { get; private set; } // 인게임 인풋 액션 구조체 프로퍼티
    public PlayerAction.UIControlActions UIActions { get; private set; } // UI 인풋 액션 구조체 프로퍼티

    private void Awake()
    {
        InputAction = new PlayerAction();
        InGameActions = InputAction.Player; // 플레이어 인풋 구성 저장
        UIActions = InputAction.UIControl;
    }

    private void OnEnable()
    {
        InputAction.Enable();
    }

    private void OnDisable()
    {
        InputAction.Disable();
    }

    public void CallDisableAction(InputAction action, float seconds)
    {
        StartCoroutine(DisableAction(action, seconds));
    }

    private IEnumerator DisableAction(InputAction action, float seconds)
    {
        action.Disable();
        yield return new WaitForSeconds(seconds);
        action.Enable();
    }
}
