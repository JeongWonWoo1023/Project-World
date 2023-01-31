using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenu : Popup
{
    [field: Header("텍스트")]
    [field: SerializeField] public TMPro.TMP_Text Name { get; private set; }

    [field: Header("버튼")]
    [field: SerializeField] public Button SetName { get; private set; }
    [field: SerializeField] public Button Inventory { get; private set; }
    [field: SerializeField] public Button Character { get; private set; }
    [field: SerializeField] public Button Map { get; private set; }
    [field: SerializeField] public Button QuitGame { get; private set; }
    [field: SerializeField] public Button Satting { get; private set; }

    private void Start()
    {
        BindQuitGameButton();
        BindInventoryButton();
    }

    public void BindQuitGameButton()
    {
        if(IsButtonNull(QuitGame))
        {
            return;
        }

        MessageFormat format = new MessageFormat("게임종료", "게임을 종료하시겠나요?");
        QuitGame.onClick.AddListener(() => UIManager.Instance.OpneMessage(format));
    }

    public void BindInventoryButton()
    {
        if (IsButtonNull(Inventory))
        {
            return;
        }
        Inventory.onClick.AddListener(() => UIManager.Instance.OpneInventory());
    }
}
