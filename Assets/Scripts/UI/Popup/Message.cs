using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct MessageFormat
{
    public string fileName;
    public string Title;
    public string Content;
    public string CancleText;
    public string OkText;
    public UnityAction CancleAction;
    public UnityAction OkAction;

    public MessageFormat(string title, string content)
    {
        fileName = "Message";
        Title = title;
        Content = content;
        CancleText = "취소";
        OkText = "확인";
        CancleAction = null;
        OkAction = null;
    }
}

public class Message : Popup
{
    [field: Header("텍스트")]
    [field: SerializeField] public TMPro.TMP_Text Title { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text Content { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text Cancle { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text OK { get; private set; }

    [field: Header("컴포넌트")]
    [field: SerializeField] public Button CancleButton { get; private set; }
    [field: SerializeField] public Button OkButton { get; private set; }
    [field: SerializeField] public Animator MessageAnimator { get; private set; }

    private bool _isOpne;
    public bool IsOpne
    {
        get => _isOpne;
        set
        {
            if(_isOpne == value)
            {
                return;
            }
            MessageAnimator.SetBool("IsOpne", value);
            _isOpne = value;
        }
    }

    public bool Initialize(MessageFormat format)
    {
        // 팝업 메세지 초기화
        bool exception = Title == null || Content == null || Cancle == null || OK == null;

        if(exception)
        {
            // 바인딩이 안되어있는 경우
            Debug.LogError($"Class : [{GetType().Name}] Object : [{gameObject.name}] Log : 텍스트 오브젝트 바인딩이 되어있는지 확인하세요");
            return false;
        }
        Title.text = format.Title;
        Content.text = format.Content;
        Cancle.text = format.CancleText;
        OK.text = format.OkText;

        CloseAction = () => 
        { 
            // 창이 닫힌경우 로직
            IsOpne = false; 
        };

        format.CancleAction = () =>
        {
            // 취소버튼 액션 로직
            UIManager.Instance.CloaePopup();
        };
        format.OkAction = () => Application.Quit();

        if (!AddCancleAction(format.CancleAction))
        {
            return false;
        }
        if(!AddOkAction(format.OkAction))
        {
            return false;
        }
        OpneAction = () =>
        {
            IsOpne = true;
        };
        return true;
    }

    public bool AddCancleAction(UnityAction action)
    {
        // 취소 버튼 이벤트 등록 메소드
        if(IsButtonNull(CancleButton))
        {
            return false;
        }

        if(FindEventAction(action.Method.Name, CancleButton))
        {
            // 대상 메소드가 이벤트에 등록되어있는 경우
            Debug.Log($"Class : [{GetType().Name}] Object : [{gameObject.name}] Log : 이벤트 메소드가 이미 등록되어있습니다\n메소드명 : [{action.Method.Name}]");
            return false;
        }
        CancleButton.onClick.AddListener(action);
        return true;
    }

    public bool AddOkAction(UnityAction action)
    {
        // 확인 버튼 이벤트 등록 메소드
        if (IsButtonNull(OkButton))
        {
            return false;
        }

        if (FindEventAction(action.Method.Name, OkButton))
        {
            // 대상 메소드가 이벤트에 등록되어있는 경우
            Debug.Log($"Class : [{GetType().Name}] Object : [{gameObject.name}] Log : 이벤트 메소드가 이미 등록되어있습니다\n메소드명 : [{action.Method.Name}]");
            return false;
        }
        OkButton.onClick.AddListener(action);
        return true;
    }
}
