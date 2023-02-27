using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingBasket : ItemEntry
{
    public TMPro.TMP_Text salePriceText;
    public Button resetButton;
    public Button saleButton;

    private int _totalPrice;
    public int TotalPrice
    {
        get => _totalPrice;
        set
        {
            _totalPrice = value <= 0 ? 0 : value;
            salePriceText.text = string.Format("{0:#,##0}", TotalPrice).ToString();
            if (UIManager.Instance.Inventory.Gold < TotalPrice)
            {
                saleButton.interactable = false;
                salePriceText.color = Color.red;
                return;
            }
            saleButton.interactable = true;
            salePriceText.color = Color.white;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        MessageFormat saleMessage = new MessageFormat();
        saleMessage.Title = "구매";
        saleMessage.Content = $"{slots[0].Item.Info.Name} {slots[0].ItemCount}개 외 {slots.Count}종류의 아이템을 구매하시겠습니까?";
        saleMessage.OkText = "구매한다";
        saleMessage.CancleText = "취소한다";
        saleMessage.OkAction = () => Sale();
        saleMessage.CancleAction = () => UIManager.Instance.ClosePopup();

        resetButton.onClick.AddListener(() => RemoveAllSlot());
        saleButton.onClick.AddListener(() => UIManager.Instance.OpneMessage(saleMessage));
    }

    private void Sale()
    {
        UIManager.Instance.ClosePopup();
        UIManager.Instance.Inventory.Gold -= TotalPrice;
        foreach(ItemSlot slot in slots)
        {
            UIManager.Instance.Inventory.AddItem(slot.Item, slot.ItemCount);
        }
    }

    public override void AddItem(Item item, int price, int count = 1)
    {
        if(count <= 0)
        {
            count = Mathf.Abs(count);
        }
        base.AddItem(item, count);
        TotalPrice += price * count;
    }

    public override void RemoveItem(Item item, int price, int count = 1)
    {
        if (count <= 0)
        {
            count = Mathf.Abs(count);
        }
        base.RemoveItem(item, count);
        TotalPrice -= price * count;
    }
}
