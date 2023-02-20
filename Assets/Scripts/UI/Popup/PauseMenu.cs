using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Popup
{
    [field: Header("텍스트")]
    [field: SerializeField] public TMPro.TMP_Text Name { get; private set; }
    [field: SerializeField] public TMPro.TMP_InputField NameField { get; private set; }

    [field: Header("버튼")]
    [field: SerializeField] public Button SetName { get; private set; }
    [field: SerializeField] public Button Inventory { get; private set; }
    [field: SerializeField] public Button Character { get; private set; }
    [field: SerializeField] public Button Map { get; private set; }
    [field: SerializeField] public Button QuitGame { get; private set; }
    [field: SerializeField] public Button Satting { get; private set; }

    private bool isNickNameInput;

    private void Start()
    {
        Name.text = UIManager.Instance.Player.nickName;
        BindQuitGameButton();
        BindInventoryButton();
        BindSettingButton();
        BindSetNameButton();
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

    public void BindSettingButton()
    {
        if(IsButtonNull(Satting))
        {
            return;
        }
        Satting.onClick.AddListener(() => UIManager.Instance.OpneSetting());
    }

    public void BindSetNameButton()
    {
        if (IsButtonNull(SetName))
        {
            return;
        }
        SetName.onClick.AddListener(
            () =>
            {
                ToggleNameField();
            });
        NameField.onEndEdit.AddListener(
            (string value) =>
            {
                Name.text = value;
                UIManager.Instance.Player.nickName = value;
                value = string.Empty;
                ToggleNameField();
            });
    }

    private void ToggleNameField()
    {
        isNickNameInput = !isNickNameInput;
        Name.gameObject.SetActive(!isNickNameInput);
        NameField.gameObject.SetActive(isNickNameInput);
    }
}
