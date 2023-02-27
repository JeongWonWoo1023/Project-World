using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMod : DropdownController
{
    private void Start()
    {
        ApplyEvent();
    }

    protected override void ApplyEvent()
    {
        // Step.1 컴포넌트 확인
        if (dropdown == null)
        {
            return;
        }

        // Step.2 세이브 데이터 확인
        int optionNumber = 0;
        if (PlayerPrefs.HasKey(saveKey))
        {
            optionNumber = PlayerPrefs.GetInt(saveKey);
        }
        else
        {
            PlayerPrefs.SetInt(saveKey, optionNumber);
        }
        dropdown.value = optionNumber;
        UpdateSoundMod(dropdown.value);

        // Step.3 메세지 정의
        MessageFormat message = new MessageFormat();
        message.Title = "안내";
        message.Content = "사운드 출력모드는 게임 재접속 후 적용됩니다";
        message.OkText = "게임 종료";
        message.OkText = "취소";
        message.OkAction = () =>
        {
            UIManager.Instance.ClosePopup();
            Application.Quit();
        };
        message.CancleAction = () =>
        {
            UIManager.Instance.ClosePopup();
            dropdown.value = optionNumber;
        };

        // Step.4 이벤트 등록
        dropdown.onValueChanged.AddListener(
            (int value) =>
            {
                UIManager.Instance.OpneMessage(message);
                PlayerPrefs.SetInt(saveKey, value);
            });
    }

    private void UpdateSoundMod(int value)
    {
        switch (value)
        {
            case 0:
                AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
                break;
            case 1:
                AudioSettings.speakerMode = AudioSpeakerMode.Mono;
                break;
            case 2:
                AudioSettings.speakerMode = AudioSpeakerMode.Surround;
                break;
        }
    }
}
