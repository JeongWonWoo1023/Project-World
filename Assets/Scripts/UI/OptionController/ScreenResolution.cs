using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : DropdownController
{
    public enum ScreenType
    {
        Full, Win1080, Win900, Win720
    }

    private ScreenType screecType;
    private void Awake()
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
        if (PlayerPrefs.HasKey(saveKey))
        {
            screecType = (ScreenType)PlayerPrefs.GetInt(saveKey);
        }
        else
        {
            PlayerPrefs.SetInt(saveKey, (int)screecType);
        }
        dropdown.value = (int)screecType;

        UpdateScreen();

        // Step.3 이벤트 등록
        dropdown.onValueChanged.AddListener(
            (int value) =>
            {
                screecType = (ScreenType)value;
                UpdateScreen();
                PlayerPrefs.SetInt(saveKey, value);
            });
    }

    private void UpdateScreen()
    {
        switch (screecType)
        {
            case ScreenType.Full:
                Screen.SetResolution(Screen.width, Screen.height, true);
                break;
            case ScreenType.Win1080:
                Screen.SetResolution(1920, 1080, false);
                break;
            case ScreenType.Win900:
                Screen.SetResolution(1600, 900, false);
                break;
            case ScreenType.Win720:
                Screen.SetResolution(1280, 720, false);
                break;
        }
    }
}
