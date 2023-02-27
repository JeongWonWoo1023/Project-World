using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    public TMPro.TMP_Text itemNameText;
    public TMPro.TMP_Text itemTypeText;
    public TMPro.TMP_Text itemDescriptionText;
    public TMPro.TMP_Text itemTooltipText;
    public Image itemIcon;
    public Transform rankTrans;

    public void Initialize(Item item = null)
    {
        // Step.1 아이템 인풋 체크
        if(item == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // Step.2 컴포넌트 초기화
        itemNameText.text = item.Info.Name;
        itemTypeText.text = item.Info.Type;
        itemDescriptionText.text = item.Info.ItemDescription;
        if (item.Info.Tooltip == string.Empty)
        {
            itemTooltipText.gameObject.SetActive(false);
        }
        else
        {
            itemTooltipText.gameObject.SetActive(true);
            itemTooltipText.text = item.Info.Tooltip;
        }
        itemIcon.sprite = item.Info.Icon;
        SetRank(item);
    }

    private void SetRank(Item item)
    {
        for(int i = 0; i < rankTrans.childCount; ++i)
        {
            if(i <= (int)item.Info.Rank)
            {
                rankTrans.GetChild(i).gameObject.SetActive(true);
                continue;
            }
            rankTrans.GetChild(i).gameObject.SetActive(false);
        }
    }
}
