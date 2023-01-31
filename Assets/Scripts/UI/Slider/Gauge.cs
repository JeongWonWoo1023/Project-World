using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// HP바, EXP바 등 게이지 UI에 적용
public class Gauge : MonoBehaviour
{
    [field: Header("구성 오브젝트 바인딩")]
    [field: SerializeField] public Slider FrontSlider { get; private set; }
    [field: SerializeField] public Slider BackSlider { get; private set; }
    [field: SerializeField] public Image FrontFillImage { get; private set; }
    [field: SerializeField] public Image BackFillImage { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ValueInfo { get; private set; }

    [field: Header("변화값")]
    [field: SerializeField] public bool Reverce { get; private set; }
    [field: SerializeField] public Gradient FillFrontGradient { get; private set; }
    [field: SerializeField] public Color FillBackColor { get; private set; }

    [field: SerializeField][field: Range(0.0f, 0.5f)] public float LerpSpeed { get; private set; } = 0.03f;

    private Coroutine coBackFill = null;

    public void UpdateFillColor()
    {
        FrontFillImage.color = FillFrontGradient.Evaluate(FrontSlider.value);
        ActiveFill(FrontSlider, FrontFillImage);
        ActiveFill(BackSlider, BackFillImage);
    }

    public void UpdateSlider()
    {
        if(!gameObject.activeSelf)
        {
            return;
        }
        if(coBackFill != null)
        {
            StopCoroutine(coBackFill);
            coBackFill = null;
        }
        coBackFill = StartCoroutine(LerpFilling());
    }

    private void ActiveFill(Slider slider,Image targetFill)
    {
        GameObject fillObj = targetFill.gameObject;
        if (slider.value > 0.001f)
        {
            if (!fillObj.activeSelf)
            {
                fillObj.SetActive(true);
            }
            return;
        }

        fillObj.SetActive(false);
    }

    private IEnumerator LerpFilling()
    {
        while (Mathf.Abs(BackSlider.value - FrontSlider.value) > 0.001f)
        {
            yield return null;
            if(Reverce)
            {
                FrontSlider.value = Mathf.Lerp(FrontSlider.value, BackSlider.value, LerpSpeed);
            }
            else
            {
                BackSlider.value = Mathf.Lerp(BackSlider.value, FrontSlider.value, LerpSpeed);
            }
        }
        BackSlider.value = FrontSlider.value;
    }
}
