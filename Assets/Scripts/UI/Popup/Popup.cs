using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public UnityAction OpneAction { get; protected set; }
    public UnityAction CloseAction { get; protected set; }

    protected bool FindEventAction(string actionName, Button button)
    {
        // 메소드가 이미 등록되어있는지 검사 메소드
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); ++i)
        {
            if (button.onClick.GetPersistentMethodName(i).Equals(actionName))
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsButtonNull(Button button)
    {
        // 버튼 바인딩 여부 체크
        if (button == null)
        {
            // 버튼 바인딩이 안되어있는 경우
            Debug.LogError($"Class : [{GetType().Name}] Object : [{gameObject.name}] Log : 버튼 오브젝트 바인딩이 되어있는지 확인하세요");
            return true;
        }
        return false;
    }
}
