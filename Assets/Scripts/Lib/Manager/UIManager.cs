using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public Enemy[] Enemys { get; private set; }
    [field: SerializeField] public Transform Canvas { get; private set; }
    [field: SerializeField] public Message MessageBox { get; private set; }
    [field: SerializeField] public Inventory Inventory { get; private set; }
    [field: SerializeField] public Shop shop { get; private set; }
    [field: SerializeField] public TalkDialog TalkDialog { get; private set; }
    [field: SerializeField] public GameSetting Setting { get; private set; }
    [field: SerializeField] public Animator UIAnimator { get; private set; }
    [field: SerializeField] public UIAnimationKeyward AnimationKeyward { get; private set; }
    [field: SerializeField] public RectTransform EnemyUIParantTrans { get; private set; }

    public string MessageFilePath { get; private set; } = "Prefabs/UI/Popup/Message";

    private Stack<Popup> _popupStack;
    public Stack<Popup> PopupStack 
    {
        get
        {
            if (_popupStack == null)
            {
                _popupStack = new Stack<Popup>();
            }
            return _popupStack;
        }
    }

    private bool _isPause;
    public bool IsPause
    {
        get => _isPause;
        set
        {
            _isPause = value;
            UIAnimator.SetBool(AnimationKeyward.PauseKey, IsPause);
        }
    }

    private bool _isOpneInventory;
    public bool IsOpneInventory
    {
        get => _isOpneInventory;
        set
        {
            _isOpneInventory = value;
            UIAnimator.SetBool(AnimationKeyward.OpneFullScreecPopupKey, IsOpneInventory);
        }
    }

    private bool _isCursor;
    public bool IsCursor
    {
        get => _isCursor;
        set
        {
            _isCursor = value;
            Cursor.visible = IsCursor;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private bool _isTalk;
    public bool IsTalk
    {
        get => _isTalk;
        set
        {
            _isTalk = value;
            IsCursor = IsTalk;
            Player.CameraInput.enabled = !IsTalk;
            UIAnimator.SetBool(AnimationKeyward.OpneTalkKey, IsTalk);
        }
    }

    private bool _isSetting;
    public bool IsSetting
    {
        get => _isSetting;
        set
        {
            _isSetting = value;
            UIAnimator.SetBool(AnimationKeyward.OpneSettingKey, IsSetting);
        }
    }

    private void Awake()
    {
        IsCursor = false;
    }

    public bool OpneMessage(MessageFormat format)
    {
        // 메세지 박스 열기
        if (MessageBox == null)
        {
            return false;
        }

        if(!MessageBox.Initialize(format))
        {
            return false;
        }

        MessageBox.OpneAction.Invoke();
        PopupStack.Push(MessageBox);
        return true;
    }

    public bool OpneInventory()
    {
        // 인벤토리 열기
        if(Inventory == null)
        {
            return false;
        }
        IsOpneInventory = true;
        PopupStack.Push(Inventory);
        return true;
    }

    public bool OpneSetting()
    {
        if(Setting == null)
        {
            return false;
        }
        IsSetting = true;
        PopupStack.Push(Setting);
        return true;
    }

    public void OpenPopup(Popup target)
    {
        target.isOpne = true;
        target.index = PopupStack.Count;
        target.Open();
        PopupStack.Push(target);
    }

    public bool ClosePopup()
    {
        // 스택 최상위에 있는 팝업 닫기
        if (_popupStack.Count == 0)
        {
            return false;
        }
        Popup target = PopupStack.Pop();
        target.CloseAction.Invoke();
        return true;
    }
}

[Serializable]
public class UIAnimationKeyward
{
    [field: SerializeField] public string PauseKey { get; private set; }
    [field: SerializeField] public string OpneFullScreecPopupKey { get; private set; }
    [field: SerializeField] public string OpneTalkKey { get; private set; }
    [field: SerializeField] public string OpneSettingKey { get; private set; }
}
