using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkDialog : MonoBehaviour
{
    public TMPro.TMP_Text Content;
    public TMPro.TMP_Text NPCName;
    public Button AutoButton;
    public Animator autoAnim;
    public Button SkipButton;
    public AudioClip textSound;
    [HideInInspector] public bool IsSkip; // 스킵 여부
    private bool _isAuto;
    public bool IsAuto // 자동 진행 여부
    {
        get => _isAuto;
        set
        {
            _isAuto = value;
            autoAnim.SetBool("Selected", IsAuto);
        }
    }
    [HideInInspector] public bool IsNext; // 다음 텍스트로 넘겼는지 여부
    Coroutine coPrintText = null;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        IsAuto = false;
        IsSkip = false;
        AutoButton.onClick.AddListener(
            () =>
            {
                IsAuto = !IsAuto;
            });
        SkipButton.onClick.AddListener(
            () =>
            {
                IsSkip = true;
            });
    }

    public IEnumerator Talk(Talk talkData, string name, Quest quest = null)
    {
        // 대화 내용 출력
        // 퀘스트 스크립트 출력
        int index = 0;
        talkData.IsEnd = false;
        UIManager.Instance.IsTalk = true;
        yield return new WaitForSeconds(0.2f);
        // 대화 로직 실행
        NPCName.text = name;
        CheckCoroutineNull(talkData.TalkText[index]);
        while (!IsSkip)
        {
            if (IsNext)
            {
                IsNext = false;
                if(++index >= talkData.TalkText.Length)
                {
                    break;
                }
                CheckCoroutineNull(talkData.TalkText[index]);
                Debug.Log($"{index} ++ {talkData.TalkText.Length}");
            }
            yield return null;
        }
        UIManager.Instance.IsTalk = false;
        if (quest != null)
        {
            quest.Start();
        }
        IsSkip = false;
        talkData.IsEnd = true;
    }

    private void CheckCoroutineNull(string text)
    {
        if (coPrintText != null)
        {
            StopCoroutine(coPrintText);
            coPrintText = null;
        }
        coPrintText = StartCoroutine(PrintText(text));
    }

    public IEnumerator PrintText(string text, float delayValue = 0.05f)
    {
        WaitForSeconds delay = new WaitForSeconds(delayValue);
        Content.text = string.Empty;
        foreach (char eliment in text)
        {
            Content.text += eliment;
            SoundManager.Instance.soundEffectSource.PlayOneShot(textSound);
            yield return delay;
        }
        if(IsAuto)
        {
            yield return new WaitForSeconds(3.0f);
            IsNext = true;
        }
    }
}
