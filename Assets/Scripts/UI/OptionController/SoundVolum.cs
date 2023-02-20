using UnityEngine;
using UnityEngine.UI;

public class SoundVolum : SliderController
{
    public int Volum { get; private set; }

    private void Awake()
    {
        ApplyEvent();
    }

    protected override void ApplyEvent()
    {
        // Step.1 컴포넌트 확인
        if(slider == null)
        {
            return;
        }

        // Step.2 세이브 데이터 확인
        if (PlayerPrefs.HasKey(saveKey))
        {
            Volum = PlayerPrefs.GetInt(saveKey);
        }
        else
        {
            Volum = 60;
            PlayerPrefs.SetInt(saveKey, Volum);
        }
        slider.value = Volum * 0.01f;
        valueText.text = Volum.ToString();
        UpdateVolum(slider.value);

        // Step.3 이벤트 등록
        slider.onValueChanged.AddListener(
            (float value) =>
            {
                UpdateVolum(value);
                Volum = (int)(value * 100);
                value = Volum * 0.01f;
                valueText.text = Volum.ToString();
                PlayerPrefs.SetInt(saveKey, Volum);
            });
    }

    private void UpdateVolum(float value)
    {
        switch (saveKey)
        {
            case "Master":
                SoundManager.Instance.mixer.SetFloat("Master", Mathf.Log10(value) * 20);
                break;
            case "BGM":
                SoundManager.Instance.mixer.SetFloat("BGM", Mathf.Log10(value) * 20);
                break;
            case "SoundEffect":
                SoundManager.Instance.mixer.SetFloat("SoundEffect", Mathf.Log10(value) * 20);
                break;
        }
    }
}
