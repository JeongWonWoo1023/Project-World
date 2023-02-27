using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Popup
{
    public TMPro.TMP_Text categoryNameText;
    public TMPro.TMP_Text gold;
    public ShoppingBasket basket;
    public ItemDescription description;
    public SaleItemList saleList;

    private void Awake()
    {
        CloseAction = () => UIManager.Instance.ClosePopup();
    }

    private void Initialize()
    {
        saleList.InitializeComponent(basket, description);
        saleList.Initialize();
        gold.text = string.Format("{0:#,##0}", UIManager.Instance.Inventory.Gold).ToString();
    }

    public override void Open()
    {
        base.Open();
        Initialize();
    }
}
