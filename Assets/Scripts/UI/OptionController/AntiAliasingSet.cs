using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntiAliasingSet : DropdownController
{
    private AntialiasingMode type = AntialiasingMode.None;

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
            type = (AntialiasingMode)PlayerPrefs.GetInt(saveKey);
        }
        else
        {
            PlayerPrefs.SetInt(saveKey, (int)type);
        }
        dropdown.value = (int)type;

        UpdateScreen();

        // Step.3 이벤트 등록
        dropdown.onValueChanged.AddListener(
        (int value) =>
        {
                type = (AntialiasingMode)value;
                UpdateScreen();
                PlayerPrefs.SetInt(saveKey, value);
            });
    }

    private void UpdateScreen()
    {
        switch (type)
        {
            case AntialiasingMode.None:
                QualitySettings.antiAliasing = (int)AntialiasingMode.None;
                break;
            case AntialiasingMode.FastApproximateAntialiasing:
                QualitySettings.antiAliasing = (int)AntialiasingMode.FastApproximateAntialiasing;
                break;
            case AntialiasingMode.SubpixelMorphologicalAntiAliasing:
                QualitySettings.antiAliasing = (int)AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
        }
    }
}
