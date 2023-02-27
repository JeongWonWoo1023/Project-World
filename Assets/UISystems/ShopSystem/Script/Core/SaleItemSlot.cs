using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaleItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public TMPro.TMP_Text itemNameText;
    public TMPro.TMP_Text limitSaleCountText;
    public TMPro.TMP_Text saleCountText;
    public TMPro.TMP_Text priceText;
    public Button addButton;
    public Button subButton;
    public Button slotButton;

    private int _saleCount;
    public int SaleCount
    {
        get => _saleCount;
        set
        {
            _saleCount = Mathf.Clamp(value,0, limit);
            saleCountText.text = SaleCount.ToString();
        }
    }

    public Item Item { get; private set; }
    private int limit;

    public void Initialize(Item item, int price, int limit)
    {
        this.limit = limit;
        priceText.text = string.Format("{0:#,##0}", price).ToString();
        Item = item;
    }

    public void InitializeEvent(UnityAction slotAction, UnityAction addAction, UnityAction subAction)
    {
        // Step.1 구성 컴포넌트 이벤트 바인딩
        addAction += () => ++SaleCount;
        subAction += () => --SaleCount;
        addButton.onClick.AddListener(addAction); // 구매 수량 증가
        subButton.onClick.AddListener(subAction); // 구매 수량 감소
        slotButton.onClick.AddListener(slotAction); // 아이템 디스크립션 출력

        // Step.2 컴포넌트 초기화
        limitSaleCountText.text = limit.ToString();
        itemNameText.text = Item.Info.Name;
        itemIcon.sprite = Item.Info.Icon;
    }
}
