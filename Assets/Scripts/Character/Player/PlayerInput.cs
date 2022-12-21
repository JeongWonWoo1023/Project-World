// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    public PlayerAction InputAction { get; private set; } // 인풋 시스템 클래스 프로퍼티
    public PlayerAction.PlayerActions Actions { get; private set; } // 인풋 액션 구조체 프로퍼티

    private void Awake()
    {
        InputAction = new PlayerAction();
        Actions = InputAction.Player; // 플레이어 인풋 구성 저장
    }

    private void OnEnable()
    {
        InputAction.Enable();
    }

    private void OnDisable()
    {
        InputAction.Disable();
    }
}
