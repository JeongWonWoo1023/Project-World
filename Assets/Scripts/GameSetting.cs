using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : Popup
{
    public Button soundMenu;
    public Button graphicMenu;
    public Button exit;
    public Transform content;

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void Initialize()
    {
        UpdateOptionSlot(OptionController.OptionCategory.Sound);
        soundMenu.onClick.AddListener(
            () =>
            {
                UpdateOptionSlot(OptionController.OptionCategory.Sound);
            });

        graphicMenu.onClick.AddListener(
            () =>
            {
                UpdateOptionSlot(OptionController.OptionCategory.Graphic);
            });

        exit.onClick.AddListener(() => UIManager.Instance.CloaePopup());

        CloseAction = () =>
        {
            // 창이 닫힌경우 로직
            UIManager.Instance.IsSetting = false;
        };
    }

    private void UpdateOptionSlot(OptionController.OptionCategory target)
    {
        for (int i = 0; i < content.childCount; ++i)
        {
            Transform child = content.GetChild(i);
            if(child.GetComponent<OptionController>().category == target)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
